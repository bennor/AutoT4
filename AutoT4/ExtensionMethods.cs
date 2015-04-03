using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using VSLangProj;

namespace BennorMcCarthy.AutoT4
{
    public static class DTEExtensions
    {
        public static IEnumerable<Project> GetProjectsWithinBuildScope(this DTE dte, vsBuildScope scope)
        {
            IEnumerable<Project> projects = null;

            switch (scope)
            {
                case vsBuildScope.vsBuildScopeSolution:
                    projects = dte.Solution.Projects.OfType<Project>();
                    break;
                case vsBuildScope.vsBuildScopeProject:
                    projects = ((object[])dte.ActiveSolutionProjects).OfType<Project>();
                    break;
            }

            return projects ?? Enumerable.Empty<Project>();
        }
    }

    public static class ProjectItemExtensions
    {
        public static IEnumerable<ProjectItem> ThatShouldRunOn(this IEnumerable<ProjectItem> projectItems, BuildEvent whenToRun)
        {
            return projectItems.Where(projectItem => projectItem.ShouldRunOn(whenToRun));
        }

        public static bool ShouldRunOn(this ProjectItem projectItem, BuildEvent whenToRun)
        {
            return (new AutoT4ProjectItemSettings(projectItem)).RunOnBuild == whenToRun;
        }

        public static void RunTemplate(this ProjectItem template)
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
    }

    public static class ProjectExtensions
    {
        public static IEnumerable<ProjectItem> FindT4ProjectItems(this IEnumerable<Project> projects)
        {
            return FindProjectItems(@"\.[Tt][Tt]$", projects);
        }

        private static IEnumerable<ProjectItem> FindProjectItems(string pattern, IEnumerable<Project> projects)
        {
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
    }
}