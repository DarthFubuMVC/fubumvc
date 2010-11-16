using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.UI.Scripts.Registration
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
            var scripts = new LinkedList<Script>();
            var script = _scripts.FirstOrDefault(s => s.Name == name);
            if(script == null)
            {
                return scripts;
            }

            gather(script, scripts);
            return scripts.Reverse();
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

        private void gather(Script script, LinkedList<Script> scripts)
        {
            script.Extensions.Each(extension => gather(extension, scripts));
            scripts.Fill(script);

            var dependencies = script.Dependencies.ToList();
            dependencies.Reverse();

            dependencies.Each(dependency =>
                                  {
                                      if(scripts.Contains(dependency))
                                      {
                                          if(scripts.Contains(script))
                                          {
                                              scripts.Remove(script);
                                          }

                                          var node = scripts.Find(dependency);
                                          scripts.AddBefore(node, script);
                                      }

                                      gather(dependency, scripts);
                                  });
        }
    }

    public static class EnumerableExtensions
    {
        public static void Fill<T>(this LinkedList<T> list, T item)
        {
            if(list.Contains(item))
            {
                return;
            }

            list.AddLast(item);
        }
    }
}