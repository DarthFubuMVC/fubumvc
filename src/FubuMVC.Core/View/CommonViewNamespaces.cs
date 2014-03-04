using System.Collections.Generic;

namespace FubuMVC.Core.View
{
    public class CommonViewNamespaces
    {
        private readonly IList<string> _namespaces = new List<string>();
        private readonly IList<string> _namespacesNotAutoImported = new List<string>();

        public void AddForType<T>()
        {
            _namespaces.Fill(typeof(T).Namespace);
        }

        public void DontAutoImportWhenNamespaceStartsWith(string namespacePrefix)
        {
            _namespacesNotAutoImported.Add(namespacePrefix);
        }

        public void Add(string @namespace)
        {
            _namespaces.Fill(@namespace);
        }

        public IEnumerable<string> Namespaces
        {
            get { return _namespaces; }
        }

        public IEnumerable<string> IgnoredNamespacesForAutoImport
        {
            get { return _namespacesNotAutoImported; }
        }
    }
}