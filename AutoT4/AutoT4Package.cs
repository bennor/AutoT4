using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BennorMcCarthy.AutoT4
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidAutoT4PkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class AutoT4Package : Package
    {
        private DTE dte;
        private BuildEvents buildEvents;

        protected override void Initialize()
        {
            base.Initialize();

            dte = GetService(typeof(SDTE)) as DTE;

            buildEvents = dte.Events.BuildEvents;
            buildEvents.OnBuildBegin += OnBuildBegin;
        }

        private void RunTemplates(params Project[] projects)
        {
            if (projects == null)
                return;

            var templates = FindProjectItems(@"\.[Tt][Tt]$", projects).ToList();
            foreach (var template in templates)
            {
                if (template.Properties.Item("ItemType").Value.ToString() == "None")
                {
                    if (!template.IsOpen)
                        template.Open();
                    template.Save();
                }
            }
        }

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            IEnumerable<Project> projects = null;
            switch (Scope)
            {
                case vsBuildScope.vsBuildScopeSolution:
                    projects = dte.Solution.Projects.OfType<Project>();
                    break;
                case vsBuildScope.vsBuildScopeProject:
                    projects = ((object[])dte.ActiveSolutionProjects).OfType<Project>();
                    break;
                default:
                    return;
            }

            RunTemplates(projects.ToArray());
        }

        private IEnumerable<ProjectItem> FindProjectItems(string pattern, IEnumerable<Project> projects)
        {
            if (projects == null)
                projects = dte.Solution.Projects.OfType<Project>();

            var regex = new Regex(pattern);
            foreach (Project project in projects)
            {
                foreach (var projectItem in FindProjectItems(regex, project.ProjectItems))
                    yield return projectItem;
            }
        }

        private static IEnumerable<ProjectItem> FindProjectItems(Regex regex, ProjectItems projectItems)
        {
            foreach (ProjectItem projectItem in projectItems)
            {
                if (regex.IsMatch(projectItem.Name ?? ""))
                    yield return projectItem;

                if (projectItem.ProjectItems != null)
                {
                    foreach (var subItem in FindProjectItems(regex, projectItem.ProjectItems))
                        yield return subItem;
                }
                if (projectItem.SubProject != null)
                {
                    foreach (var subItem in FindProjectItems(regex, projectItem.SubProject.ProjectItems))
                        yield return subItem;
                }
            }
        }

        protected override int QueryClose(out bool canClose)
        {
            int result = base.QueryClose(out canClose);
            if (!canClose)
                return result;

            if (buildEvents != null)
            {
                buildEvents.OnBuildBegin -= OnBuildBegin;
                buildEvents = null;
            }
            return result;
        }
    }
}
