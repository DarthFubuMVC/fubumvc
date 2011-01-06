using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.UI.Scripts
{
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

            _scripts.Sort(_graph);

            return _scripts;
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
}