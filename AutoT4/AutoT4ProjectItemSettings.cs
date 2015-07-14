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
        private const RunOnBuild DefaultRunOnBuildSetting = RunOnBuild.Default;

        public AutoT4ProjectItemSettings(ProjectItem item)
            : base(item, "AutoT4") { }

        [DefaultValue(DefaultRunOnBuildSetting)]
        [DisplayName("Run on build")]
        [Category("AutoT4")]
        [Description("Whether to run this template at build time or not.")]
        [TypeConverter(typeof(EnumDescriptionConverter))]
        public RunOnBuild RunOnBuild
        {
            get { return Get(DefaultRunOnBuildSetting, CoerceOldRunOnBuildValue); }
            set { Set(value); }
        }

        /// <summary>
        /// Converts the old <see cref="bool"/> <see cref="RunOnBuild"/> property to <see cref="RunOnBuild"/>
        /// </summary>
        private RunOnBuild CoerceOldRunOnBuildValue(string value)
        {
            var newRunOnBuildValue = DefaultRunOnBuildSetting;
            bool previousRunOnBuild;
            if (bool.TryParse(value, out previousRunOnBuild))
            {
                newRunOnBuildValue = previousRunOnBuild
                                         ? RunOnBuild.Default
                                         : RunOnBuild.Disabled;
            }

            // Coercion was needed, therefore the new value needs to be assigned so that it gets migrated in the settings
            RunOnBuild = newRunOnBuildValue;

            return newRunOnBuildValue;
        }
    }
}
