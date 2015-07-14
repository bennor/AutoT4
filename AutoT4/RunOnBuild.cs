using System.ComponentModel;

namespace BennorMcCarthy.AutoT4
{
    public enum RunOnBuild
    {
        [Description("Default")]
        Default = -1,
        Disabled = 0,
        [Description("Before build")]
        BeforeBuild,
        [Description("After build")]
        AfterBuild
    }
}