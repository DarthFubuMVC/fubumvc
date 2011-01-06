using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.UI.Scripts
{
    public class ScriptSet : ScriptObjectBase
    {
        private readonly List<string> _includes = new List<string>();
        private IList<IScriptObject> _objects;

        public override IEnumerable<IScript> AllScripts()
        {
            return _objects.SelectMany(x => x.AllScripts()).Distinct();
        }

        public void FindScripts(ScriptGraph graph)
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

        public void Add(IScriptObject child)
        {
            if (_objects == null)
            {
                _objects = new List<IScriptObject>();
            }

            _objects.Add(child);
        }
    }
}