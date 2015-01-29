using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
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
using VSLangProj;

namespace BennorMcCarthy.AutoT4
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidAutoT4PkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class AutoT4Package : Package
    {
        private DTE _dte;
        private BuildEvents _buildEvents;
        private ObjectExtenders _objectExtenders;
        private AutoT4ExtenderProvider _extenderProvider;
        private readonly List<int> _extenderProviderCookies = new List<int>();

        protected override void Initialize()
        {
            base.Initialize();

            _dte = GetService(typeof(SDTE)) as DTE;
            if (_dte == null)
                return;

            RegisterExtenderProvider(VSConstants.CATID.CSharpFileProperties_string);
            RegisterExtenderProvider(VSConstants.CATID.VBFileProperties_string);

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _buildEvents = _dte.Events.BuildEvents;
            _buildEvents.OnBuildBegin += OnBuildBegin;
        }

        private void RegisterExtenderProvider(string catId)
        {
            const string name = AutoT4ExtenderProvider.Name;

            _objectExtenders = _objectExtenders ?? GetService(typeof(ObjectExtenders)) as ObjectExtenders;
            if (_objectExtenders == null)
                return;

            _extenderProvider = _extenderProvider ?? new AutoT4ExtenderProvider(_dte);
            _extenderProviderCookies.Add(_objectExtenders.RegisterExtenderProvider(catId, name, _extenderProvider));
        }

        private void RunTemplates(params Project[] projects)
        {
            if (projects == null)
                return;

            var templates = FindProjectItems(@"\.[Tt][Tt]$", projects).ToList();
            foreach (var template in templates)
            {
                if(new AutoT4ProjectItemSettings(template).RunOnBuild)
                    RunTemplate(template);
            }
        }

        private void RunTemplate(ProjectItem template)
        {
            var templateVsProjectItem = template.Object as VSProjectItem;
            if (templateVsProjectItem != null)
            {
                templateVsProjectItem.RunCustomTool();
            }
            else
            {
                if (!template.IsOpen)
                    template.Open();
                template.Save();
            }
        }

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            IEnumerable<Project> projects = null;
            switch (Scope)
            {
                case vsBuildScope.vsBuildScopeSolution:
                    projects = _dte.Solution.Projects.OfType<Project>();
                    break;
                case vsBuildScope.vsBuildScopeProject:
                    projects = ((object[])_dte.ActiveSolutionProjects).OfType<Project>();
                    break;
                default:
                    return;
            }

            RunTemplates(projects.ToArray());
        }

        private IEnumerable<ProjectItem> FindProjectItems(string pattern, IEnumerable<Project> projects)
        {
            if (projects == null)
                projects = _dte.Solution.Projects.OfType<Project>();

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

            if (_buildEvents != null)
            {
                _buildEvents.OnBuildBegin -= OnBuildBegin;
                _buildEvents = null;
            }
            return result;
        }
    }
}
