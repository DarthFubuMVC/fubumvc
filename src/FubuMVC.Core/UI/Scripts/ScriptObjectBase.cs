using System.Collections.Generic;

namespace FubuMVC.Core.UI.Scripts
{
    public abstract class ScriptObjectBase : IScriptObject
    {
        private readonly List<string> _aliases = new List<string>();

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

        public abstract IEnumerable<IScript> AllScripts(ScriptGraph graph);
    }
}