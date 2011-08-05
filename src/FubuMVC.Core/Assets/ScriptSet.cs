using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Assets
{
    public class ScriptSet : AssetBase
    {
        private readonly List<string> _includes = new List<string>();
        private IList<IAsset> _objects;

        public override IEnumerable<IAssetDependency> AllScripts()
        {
            return _objects.SelectMany(x => x.AllScripts()).Distinct();
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

        public void Add(IAsset child)
        {
            if (_objects == null)
            {
                _objects = new List<IAsset>();
            }

            _objects.Add(child);
        }

        public override string ToString()
        {
            return "Set:  " + Name;
        }
    }
}