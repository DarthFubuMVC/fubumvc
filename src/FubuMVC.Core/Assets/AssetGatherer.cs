using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Assets
{

    public class AssetLevel
    {
        public IFileDependency FileDependency { get; set; }
        public int Level { get; set; }
    }


    public class AssetGatherer
    {
        private readonly AssetGraph _graph;
        private readonly IEnumerable<string> _names;
        private readonly List<IFileDependency> _scripts = new List<IFileDependency>();

        public AssetGatherer(AssetGraph graph, IEnumerable<string> names)
        {
            _graph = graph;
            _names = names;
        }

        public IEnumerable<IFileDependency> Gather()
        {
            _names.Select(x => _graph.ObjectFor(x)).Distinct().Each(gatherFrom);

            var sorter = new ScriptSorter(_scripts);

            return sorter.Sort();
        }



        private readonly IList<IRequestedAsset> _gatheredList = new List<IRequestedAsset>();

        private void gatherFrom(IRequestedAsset asset)
        {
            if (_gatheredList.Contains(asset)) return;
            _gatheredList.Add(asset);

            var allScripts = asset.AllFileDependencies();
            _scripts.Fill(allScripts);

            allScripts.Each(gatherFrom);
            asset.Dependencies().Each(gatherFrom);
        }
    }


    public class ScriptSorter
    {
        private readonly IList<IFileDependency> _scripts;
        private readonly IList<IList<IFileDependency>> _levels = new List<IList<IFileDependency>>();

        public ScriptSorter(IList<IFileDependency> scripts)
        {
            _scripts = scripts;
        }

        public IEnumerable<IFileDependency> Sort()
        {
            var top = _scripts.Where(x => x.IsFirstRank()).ToList();
            _scripts.RemoveAll(top.Contains);
            _levels.Add(top);

            while (_scripts.Any())
            {
                var level = new List<IFileDependency>();
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


