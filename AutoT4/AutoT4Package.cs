using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System;

namespace BennorMcCarthy.AutoT4
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidAutoT4PkgString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    [ProvideOptionPage(typeof(Options), Options.CategoryName, Options.PageName, 1000, 1001, false)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class AutoT4Package : Package
    {
        private const int CommandId = 0x0100;
        private static readonly Guid CommandSet = new Guid("92c31ba2-5827-4779-b3ff-cf0fed43e50a");

        private DTE _dte;
        private BuildEvents _buildEvents;
        private ObjectExtenders _objectExtenders;
        private AutoT4ExtenderProvider _extenderProvider;
        private readonly List<int> _extenderProviderCookies = new List<int>();

        private Options Options
        {
            get { return (Options)GetDialogPage(typeof(Options)); }
        }

        protected override void Initialize()
        {
            base.Initialize();

            _dte = GetService(typeof(SDTE)) as DTE;
            if (_dte == null)
                return;

            RegisterExtenderProvider(VSConstants.CATID.CSharpFileProperties_string);
            RegisterExtenderProvider(VSConstants.CATID.VBFileProperties_string);

            RegisterEvents();

            var commandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.RunTemplates, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        private void RegisterEvents()
        {
            _buildEvents = _dte.Events.BuildEvents;
            _buildEvents.OnBuildBegin += OnBuildBegin;
            _buildEvents.OnBuildDone += OnBuildDone;
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

        private void OnBuildBegin(vsBuildScope scope, vsBuildAction action)
        {
            RunTemplates(scope, RunOnBuild.BeforeBuild, Options.RunOnBuild == DefaultRunOnBuild.BeforeBuild);
        }

        private void OnBuildDone(vsBuildScope scope, vsBuildAction action)
        {
            RunTemplates(scope, RunOnBuild.AfterBuild, Options.RunOnBuild == DefaultRunOnBuild.AfterBuild);
        }

        private void RunTemplates(vsBuildScope scope, RunOnBuild buildEvent, bool runIfDefault)
        {
            _dte.GetProjectsWithinBuildScope(scope)
                .FindT4ProjectItems()
                .ThatShouldRunOn(buildEvent, runIfDefault)
                .ToList()
                .ForEach(item => item.RunTemplate());
        }

        private void RunTemplates(object sender, EventArgs e)
        {
            _dte.GetProjectsWithinBuildScope(vsBuildScope.vsBuildScopeSolution)
                .FindT4ProjectItems()
                .ToList()
                .ForEach(item => item.RunTemplate());
        }

        protected override int QueryClose(out bool canClose)
        {
            int result = base.QueryClose(out canClose);
            if (!canClose)
                return result;

            if (_buildEvents != null)
            {
                _buildEvents.OnBuildBegin -= OnBuildBegin;
                _buildEvents.OnBuildDone -= OnBuildDone;
                _buildEvents = null;
            }
            return result;
        }
    }
}
