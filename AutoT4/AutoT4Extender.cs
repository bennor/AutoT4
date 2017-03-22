using EnvDTE;
using System;
using System.Runtime.InteropServices;

namespace BennorMcCarthy.AutoT4
{
    [CLSCompliant(false)]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class AutoT4Extender : AutoT4ProjectItemSettings
    {
        private readonly IExtenderSite _extenderSite;
        private readonly int _cookie;

        public AutoT4Extender(ProjectItem item, IExtenderSite extenderSite, int cookie)
            : base(item)
        {
            if (extenderSite == null)
                throw new ArgumentNullException("extenderSite");

            _extenderSite = extenderSite;
            _cookie = cookie;
        }

        ~AutoT4Extender()
        {
            try
            {
                if (_extenderSite != null)
                    _extenderSite.NotifyDelete(_cookie);
            }
            catch
            {
            }
        }
    }
}