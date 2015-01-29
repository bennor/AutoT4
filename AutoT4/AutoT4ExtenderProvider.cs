using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace BennorMcCarthy.AutoT4
{
    public class AutoT4ExtenderProvider : IExtenderProvider
    {
        public const string Name = "AutoT4ExtenderProvider";

        private readonly DTE _dte;

        public AutoT4ExtenderProvider(DTE dte)
        {
            if (dte == null)
                throw new ArgumentNullException("dte");

            _dte = dte;
        }

        public object GetExtender(string extenderCatId, string extenderName, object extendeeObject, IExtenderSite extenderSite, int cookie)
        {
            if (!CanExtend(extenderCatId, extenderName, extendeeObject))
                return null;

            var fileProperties = extendeeObject as VSLangProj.FileProperties;
            if (fileProperties == null)
                return null;

            var item = _dte.Solution.FindProjectItem(fileProperties.FullPath);
            if (item == null)
                return null;

            return new AutoT4Extender(item, extenderSite, cookie);
        }

        public bool CanExtend(string extenderCatid, string extenderName, object extendeeObject)
        {
            var fileProperties = extendeeObject as VSLangProj.FileProperties;
            return extenderName == Name &&
                   fileProperties != null &&
                   ".tt".Equals(fileProperties.Extension, StringComparison.OrdinalIgnoreCase);
        }
    }
}