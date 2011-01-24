using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FubuMVC.Core.UI.Scripts
{

    public class ScriptLevel
    {
        public IScript Script { get; set; }
        public int Level { get; set; }
    }


    public class ScriptGatherer
    {
        private readonly ScriptGraph _graph;
        private readonly IEnumerable<string> _names;
        private readonly List<IScript> _scripts = new List<IScript>();

        public ScriptGatherer(ScriptGraph graph, IEnumerable<string> names)
        {
            _graph = graph;
            _names = names;
        }

        public IEnumerable<IScript> Gather()
        {
            _names.Select(x => _graph.ObjectFor(x)).Distinct().Each(gatherFrom);

            var sorter = new ScriptSorter(_scripts);

            

            return sorter.Sort();
        }



        private readonly IList<IScriptObject> _gatheredList = new List<IScriptObject>();

        private void gatherFrom(IScriptObject scriptObject)
        {
            if (_gatheredList.Contains(scriptObject)) return;
            _gatheredList.Add(scriptObject);

            var allScripts = scriptObject.AllScripts();
            _scripts.Fill(allScripts);

            allScripts.Each(gatherFrom);
            scriptObject.Dependencies().Each(gatherFrom);
        }
    }


    public class ScriptSorter
    {
        private readonly IList<IScript> _scripts;
        private readonly IList<IList<IScript>> _levels = new List<IList<IScript>>();

        public ScriptSorter(IList<IScript> scripts)
        {
            _scripts = scripts;
        }

        public IEnumerable<IScript> Sort()
        {
            var top = _scripts.Where(x => x.IsFirstRank()).ToList();
            _scripts.RemoveAll(top.Contains);
            _levels.Add(top);

            while (_scripts.Any())
            {
                var level = new List<IScript>();
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


