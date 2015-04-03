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

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            RunTemplates(Scope, BuildEvent.BeforeBuild);
        }

        private void OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            RunTemplates(Scope, BuildEvent.AfterBuild);
        }

        private void RunTemplates(vsBuildScope scope, BuildEvent buildEvent)
        {
            _dte.GetProjectsWithinBuildScope(scope)
                .FindT4ProjectItems()
                .ThatShouldRunOn(buildEvent)
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
