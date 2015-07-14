using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace BennorMcCarthy.AutoT4
{
    public class Options : DialogPage
    {
        private DefaultRunOnBuild _buildAction = DefaultRunOnBuild.BeforeBuild;

        public const string CategoryName = "AutoT4";
        public const string PageName = "General";

        [Category("General")]
        [DisplayName("Run on build")]
        [Description("Run T4 templates when building.")]
        [DefaultValue(DefaultRunOnBuild.BeforeBuild)]
        [TypeConverter(typeof(EnumDescriptionConverter))]
        public DefaultRunOnBuild RunOnBuild
        {
            get { return _buildAction; }
            set { _buildAction = value; }
        }
    }
}