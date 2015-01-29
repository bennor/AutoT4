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
            : base(item, "AutoT4") {}

        [DefaultValue(true)]
        [DisplayName("Run on build")]
        [Category("AutoT4")]
        [Description("Whether to run this template at build time or not.")]
        public bool RunOnBuild
        {
            get { return Get(true); }
            set { Set(value); }
        }
    }
}
