using System.ComponentModel;

namespace BennorMcCarthy.AutoT4
{
    public enum DefaultRunOnBuild
    {
        Disabled,
        [Description("Before build")]
        BeforeBuild,
        [Description("After build")]
        AfterBuild
    }
}