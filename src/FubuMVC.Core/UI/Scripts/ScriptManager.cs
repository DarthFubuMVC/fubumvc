using System.Collections.Generic;
using FubuMVC.UI.Scripts.Registration;

namespace FubuMVC.UI.Scripts
{
    public class ScriptManager : IScriptManager
    {
        private readonly ScriptGraph _graph;
        private readonly List<string> _includes;
        private List<Script> _scripts;

        public ScriptManager(ScriptGraph graph)
        {
            _graph = graph;
            _includes = new List<string>();
        }

        public void RegisterInclude(string path)
        {
            _includes.Add(path);
            _scripts = null;
        }

        public IEnumerable<Script> RegisteredScripts()
        {
            if(_scripts == null)
            {
                _scripts = new List<Script>();
                _includes
                    .Each(path => _scripts.AddRange(_graph.GetScript(path)));
            }

            return _scripts;
        }
    }
}