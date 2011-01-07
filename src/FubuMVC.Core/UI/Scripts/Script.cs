using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using HtmlTags;

namespace FubuMVC.Core.UI.Scripts
{
    public class ScriptFile
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public string ReadAll()
        {
            throw new NotImplementedException();
        }
        public HtmlTag CreateScriptTag()
        {
            throw new NotImplementedException();
        }
    }

    public class Script : ScriptObjectBase, IScript
    {
        private readonly Cache<IScript, bool> _isAfter = new Cache<IScript, bool>();

        public Script()
        {
            _isAfter.OnMissing = searchForDependency;
        }

        public Script(string name) : this()
        {
            Name = name;
        }

        public override IEnumerable<IScript> AllScripts()
        {
            yield return this;
        }

        public bool ShouldBeAfter(IScript script)
        {
            return _isAfter[script];
        }

        public void OrderedAfter(IScript script)
        {
            _isAfter[script] = true;
        }

        public void OrderedBefore(IScript script)
        {
            _isAfter[script] = false;
        }

        private bool searchForDependency(IScript script)
        {
            return this.Any(x => x == script);
        }
    }
}