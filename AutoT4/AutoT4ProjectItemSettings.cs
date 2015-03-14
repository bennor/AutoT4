using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using EnvDTE;

namespace BennorMcCarthy.AutoT4
{
    [CLSCompliant(false)]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class AutoT4ProjectItemSettings : ProjectItemSettings
    {
        public AutoT4ProjectItemSettings(ProjectItem item)
            : base(item, "AutoT4") { }

        [DefaultValue(BuildEvent.BeforeBuild)]
        [DisplayName("Run on build")]
        [Category("AutoT4")]
        [Description("Whether to run this template at build time or not.")]
        public BuildEvent RunOnBuild
        {
            get { return Get(BuildEvent.BeforeBuild); }
            set { Set(value); }
        }
    }

    public enum BuildEvent
    {
        DoNotRun,
        BeforeBuild,
        AfterBuild
    }
}
