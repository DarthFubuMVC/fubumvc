using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Assets
{
    public class AssetSet : AssetBase
    {
        private readonly List<string> _includes = new List<string>();
        private IList<IRequestedAsset> _objects;

        public override IEnumerable<IFileDependency> AllFileDependencies()
        {
            if (_objects == null) return Enumerable.Empty<IFileDependency>();

            return _objects.SelectMany(x => x.AllFileDependencies()).Distinct();
        }

        public void FindScripts(AssetGraph graph)
        {
            if (_objects == null)
            {
                _objects = _includes.Select(graph.ObjectFor).ToList();
            }
        }

        public void Add(string name)
        {
            _includes.Fill(name);
        }

        public void Add(IRequestedAsset child)
        {
            if (_objects == null)
            {
                _objects = new List<IRequestedAsset>();
            }

            _objects.Add(child);
        }

        public override string ToString()
        {
            return "Set:  " + Name;
        }
    }
}