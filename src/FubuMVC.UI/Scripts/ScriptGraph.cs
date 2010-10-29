using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.UI.Scripts
{
    public class ScriptGraph
    {
        private readonly List<ScriptSet> _sets;
        private readonly List<Script> _scripts;

        public ScriptGraph()
        {
            _sets = new List<ScriptSet>();
            _scripts = new List<Script>();
            
        }

        public ScriptSet GetScriptSet(string name)
        {
            return _sets.FirstOrDefault(set => set.Name == name);
        }

        public IEnumerable<Script> GetScript(string name)
        {
            var scripts = new List<Script>();
            var script = _scripts.FirstOrDefault(s => s.Name == name);
            if(script == null)
            {
                return scripts;
            }

            gather(script, scripts);
            scripts.Reverse();

            return scripts;
        }

        private void gather(Script script, List<Script> scripts)
        {
            script.Extensions.Each(extension => gather(extension, scripts));
            scripts.Add(script);

            script.Dependencies.Each(dependency => gather(dependency, scripts));
        }

        public Script RegisterScript(string name, string path)
        {
            var script = new Script(name, path);
            var defaultSet = GetScriptSet(name) ?? new ScriptSet(name);
            
            defaultSet.AddScript(script);
            _sets.Fill(defaultSet);
            return script;
        }

        public void ReplaceScript(Script original, Script newScript)
        {
            
        }
    }
}