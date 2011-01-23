using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.UI.Scripts
{
    public class Script : ScriptObjectBase, IScript
    {
        private readonly IList<IScript> _extensions = new List<IScript>();
        private readonly Cache<IScript, bool> _isAfter = new Cache<IScript, bool>();

        public Script()
        {
            _isAfter.OnMissing = isAfterInChain;
        }

        public Script(string name) : this()
        {
            Name = name;
        }

        public override IEnumerable<IScript> AllScripts()
        {
            yield return this;

            foreach (var extension in _extensions)
            {
                yield return extension;
            }
        }

        public bool DependsOn(IScript script)
        {
            var returnValue = _isAfter[script];
            return returnValue;
        }

        public void AddExtension(IScript extender)
        {
            _extensions.Add(extender);
            _isAfter[extender] = false;
        }

        public bool HasDependencies()
        {
            return Dependencies().Any();
        }

        public int CompareTo(IScript other)
        {
            return Name.CompareTo(other.Name);
        }

        private bool isAfterInChain(IScript script)
        {
            // The filter on "not this" is introduced because of the extensions
            var dependencies = Dependencies().SelectMany(x => x.AllScripts()).Where(x => !ReferenceEquals(x, this));
            return dependencies.Contains(script) || dependencies.Any(x => x.DependsOn(script));
        }

        public override string ToString()
        {
            return string.Format("Script: {0}", Name);
        }
    }
}