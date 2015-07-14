using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using VSLangProj;

namespace BennorMcCarthy.AutoT4
{
    public static class DTEExtensions
    {
        private const string SolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

        public static IEnumerable<Project> GetProjectsWithinBuildScope(this DTE dte, vsBuildScope scope)
        {
            IEnumerable<Project> projects = null;

            switch (scope)
            {
                case vsBuildScope.vsBuildScopeSolution:
                    projects = GetProjectsInSolution(dte.Solution).Where(x => x.ProjectItems != null);
                    break;
                case vsBuildScope.vsBuildScopeProject:
                    projects = ((object[])dte.ActiveSolutionProjects).OfType<Project>();
                    break;
            }

            return projects ?? Enumerable.Empty<Project>();
        }

        private static IEnumerable<Project> GetProjectsInSolution(Solution solution)
        {
            foreach (Project project in solution.Projects.OfType<Project>())
            {
                if (project.Kind == SolutionFolder)
                {
                    foreach (Project folderProject in GetProjectsInSolutionFolder(project).Where(x => x.Kind != SolutionFolder))
                    {
                        yield return folderProject;
                    }
                }
                else
                {
                    yield return project;
                }
            }
        }

        private static IEnumerable<Project> GetProjectsInSolutionFolder(Project solutionFolderProject)
        {
            if (solutionFolderProject.ProjectItems != null)
            {
                foreach (ProjectItem projectItem in solutionFolderProject.ProjectItems)
                {
                    Project subProject = projectItem.SubProject as Project;

                    if (subProject != null)
                    {
                        if (subProject.Kind == SolutionFolder)
                        {
                            foreach (Project folderProject in GetProjectsInSolutionFolder(subProject).Where(x => x.Kind != SolutionFolder))
                            {
                                yield return folderProject;
                            }
                        }
                        else
                        {
                            yield return subProject;
                        }
                    }
                }
            }
        }
    }

    public static class ProjectItemExtensions
    {
        public static IEnumerable<ProjectItem> ThatShouldRunOn(this IEnumerable<ProjectItem> projectItems, RunOnBuild whenToRun, bool runIfDefault)
        {
            return projectItems.Where(projectItem => projectItem.ShouldRunOn(whenToRun, runIfDefault));
        }

        public static bool ShouldRunOn(this ProjectItem projectItem, RunOnBuild whenToRun, bool runIfDefault)
        {
            var setting = new AutoT4ProjectItemSettings(projectItem).RunOnBuild;
            return setting == whenToRun ||
                   (setting == RunOnBuild.Default && runIfDefault);
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