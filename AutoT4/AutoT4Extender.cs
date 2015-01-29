using EnvDTE;
using System;
using System.Runtime.InteropServices;

namespace BennorMcCarthy.AutoT4
{
    [CLSCompliant(false)]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public sealed class AutoT4Extender : AutoT4ProjectItemSettings, IDisposable
    {
        private readonly IExtenderSite _extenderSite;
        private readonly int _cookie;
        private bool _disposed;

        public AutoT4Extender(ProjectItem item, IExtenderSite extenderSite, int cookie)
            :base(item)
        {
            if (extenderSite == null) 
                throw new ArgumentNullException("extenderSite");

            _extenderSite = extenderSite;
            _cookie = cookie;
        }

        ~AutoT4Extender()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            // take the instance off of the finalization queue.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            _disposed = true;

            if (!disposing || _cookie == 0)
                return;

            _extenderSite.NotifyDelete(_cookie);
        }
    }
}