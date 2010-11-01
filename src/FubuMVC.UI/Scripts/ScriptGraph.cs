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

        public Script RegisterScript(string name, string path)
        {
            var script = new Script(name, path);
            _scripts.Fill(script);
            return script;
        }

        public ScriptSet RegisterSet(string name)
        {
            var set = GetScriptSet(name) ?? new ScriptSet(name);
            _sets.Fill(set);

            return set;
        }

        public void ReplaceScript(string name, Script newScript)
        {
            var script = GetScript(name).FirstOrDefault(s => s.Name == name);
            if(script == null)
            {
                return;
            }

            script.ResetPath(newScript.Path);
        }

        private void gather(Script script, List<Script> scripts)
        {
            script.Extensions.Each(extension => gather(extension, scripts));
            scripts.Add(script);

            var dependencies = script.Dependencies.ToList();
            dependencies.Reverse();

            dependencies.Each(dependency => gather(dependency, scripts));
        }
    }
}