using System;
using System.Collections.Generic;

namespace FubuMVC.Core.UI.Scripts
{
    public abstract class ScriptObjectBase : IScriptObject
    {
        private readonly List<string> _aliases = new List<string>();
        private readonly List<IScriptObject> _dependencies = new List<IScriptObject>();

        public bool Matches(string key)
        {
            var keyToMatch = key.ToLowerInvariant();
            return Name.ToLowerInvariant() == keyToMatch || _aliases.Contains(keyToMatch);
        }

        public string Name { get; set; }

        public void AddAlias(string alias)
        {
            _aliases.Fill(alias.ToLowerInvariant());
        }

        public abstract IEnumerable<IScript> AllScripts();

        public IEnumerable<IScriptObject> Dependencies()
        {
            return _dependencies;
        }

        public void AddDependency(IScriptObject scriptObject)
        {
            _dependencies.Fill(scriptObject);
        }
    }
}