using System;
using System.Collections.Generic;
using HtmlTags;

namespace FubuMVC.Core.UI.Scripts
{
    public class ScriptProxy : ScriptObjectBase, IScript
    {
        private readonly Lazy<IScript> _inner;

        public ScriptProxy(string name, IScriptFinder finder)
        {
            Name = name;
            _inner = new Lazy<IScript>(() => finder.Find(name));
        }

        public override IEnumerable<IScript> AllScripts()
        {
            return _inner.Value.AllScripts();
        }

        public string ReadAll()
        {
            return _inner.Value.ReadAll();
        }

        public HtmlTag CreateScriptTag()
        {
            return _inner.Value.CreateScriptTag();
        }

        public bool ShouldBeAfter(IScript script)
        {
            throw new NotImplementedException();
        }
    }
}