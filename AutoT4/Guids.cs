// Guids.cs
// MUST match guids.h
using System;

namespace BennorMcCarthy.AutoT4
{
    static class GuidList
    {
        public const string guidAutoT4PkgString = "3fe81b73-39c9-4e39-8742-8e3fc01493bd";
        public const string guidAutoT4CmdSetString = "1b1b018a-07fd-4501-bac2-55a65d4971fe";

        public static readonly Guid guidAutoT4CmdSet = new Guid(guidAutoT4CmdSetString);
    };
}