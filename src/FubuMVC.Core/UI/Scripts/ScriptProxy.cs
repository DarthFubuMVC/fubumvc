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

        public override IEnumerable<IScript> AllScripts(ScriptGraph graph)
        {
            return _inner.Value.AllScripts(graph);
        }

        public string ReadAll()
        {
            return _inner.Value.ReadAll();
        }

        public HtmlTag CreateScriptTag()
        {
            return _inner.Value.CreateScriptTag();
        }
    }
}