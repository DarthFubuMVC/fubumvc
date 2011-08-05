using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Assets
{

    public class ScriptLevel
    {
        public IAssetDependency AssetDependency { get; set; }
        public int Level { get; set; }
    }


    public class ScriptGatherer
    {
        private readonly AssetGraph _graph;
        private readonly IEnumerable<string> _names;
        private readonly List<IAssetDependency> _scripts = new List<IAssetDependency>();

        public ScriptGatherer(AssetGraph graph, IEnumerable<string> names)
        {
            _graph = graph;
            _names = names;
        }

        public IEnumerable<IAssetDependency> Gather()
        {
            _names.Select(x => _graph.ObjectFor(x)).Distinct().Each(gatherFrom);

            var sorter = new ScriptSorter(_scripts);

            

            return sorter.Sort();
        }



        private readonly IList<IAsset> _gatheredList = new List<IAsset>();

        private void gatherFrom(IAsset asset)
        {
            if (_gatheredList.Contains(asset)) return;
            _gatheredList.Add(asset);

            var allScripts = asset.AllScripts();
            _scripts.Fill(allScripts);

            allScripts.Each(gatherFrom);
            asset.Dependencies().Each(gatherFrom);
        }
    }


    public class ScriptSorter
    {
        private readonly IList<IAssetDependency> _scripts;
        private readonly IList<IList<IAssetDependency>> _levels = new List<IList<IAssetDependency>>();

        public ScriptSorter(IList<IAssetDependency> scripts)
        {
            _scripts = scripts;
        }

        public IEnumerable<IAssetDependency> Sort()
        {
            var top = _scripts.Where(x => x.IsFirstRank()).ToList();
            _scripts.RemoveAll(top.Contains);
            _levels.Add(top);

            while (_scripts.Any())
            {
                var level = new List<IAssetDependency>();
                foreach (var script in _scripts.ToArray())
                {
                    if (!_scripts.Any(x => script.MustBeAfter(x)))
                    {
                        level.Add(script);
                    }
                }

                _levels.Add(level);
                _scripts.RemoveAll(level.Contains);
            }

            return _levels.SelectMany(x => x.OrderBy(y => y.Name));
        }
    }
}


