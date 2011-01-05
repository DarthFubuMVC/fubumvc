using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.UI.Scripts
{
    public class ScriptSet : ScriptObjectBase
    {
        private readonly List<string> _includes = new List<string>();

        public override IEnumerable<IScript> AllScripts(ScriptGraph graph)
        {
            return _includes.SelectMany(x => graph.ObjectFor(x).AllScripts(graph)).Distinct();
        }

        public void Add(string name)
        {
            _includes.Fill(name);
        }
    }
}