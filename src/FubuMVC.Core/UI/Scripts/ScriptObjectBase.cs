using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerator<IScript> GetEnumerator()
        {
            foreach (IScript script in AllScripts())
            {
                yield return script;

                foreach (var dependency in script.Dependencies())
                {
                    foreach (var s in dependency)
                    {
                        yield return s;
                    }
                }
            }

            foreach (var scriptObject in _dependencies)
            {
                foreach (IScript s in scriptObject)
                {
                    yield return s;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}