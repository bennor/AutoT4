using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace BennorMcCarthy.AutoT4
{
    [CLSCompliant(false)]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ProjectItemSettings
    {
        private readonly string _scope;
        private readonly IVsBuildPropertyStorage _storage;
        private readonly uint _itemId;
        private readonly bool _isInitialized;

        public ProjectItemSettings(ProjectItem item, string scope = null)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if(!string.IsNullOrWhiteSpace(scope))
                _scope = scope.Trim();

            var solution = (IVsSolution) Package.GetGlobalService(typeof (SVsSolution));

            IVsHierarchy hierarchy;
            if (0 != solution.GetProjectOfUniqueName(item.ContainingProject.UniqueName, out hierarchy))
                return;

            _storage = hierarchy as IVsBuildPropertyStorage;
            if (_storage == null)
                return;

            var fullPath = item.FileNames[0];
                
            if (0 != hierarchy.ParseCanonicalName(fullPath, out _itemId))
                return;

            _isInitialized = true;
        }

        protected void Set<T>(T value = default(T), [CallerMemberName] string name = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (!_isInitialized)
                return;

            var serializedValue = Convert.ToString(value);
            _storage.SetItemAttribute(_itemId, Scope(name), serializedValue);
        }

        protected T Get<T>(T defaultValue = default(T), [CallerMemberName] string name = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (!_isInitialized)
                return defaultValue;

            string serializedValue;
            if (0 != _storage.GetItemAttribute(_itemId, Scope(name), out serializedValue))
                return defaultValue;

            return !String.IsNullOrWhiteSpace(serializedValue)
                       ? (T)Convert.ChangeType(serializedValue, typeof(T))
                       : defaultValue;
        }

        private string Scope(string name)
        {
            return _scope != null
                       ? string.Format("{0}{1}", _scope, name)
                       : name;
        }
    }
}