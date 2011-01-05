using System.Collections.Generic;

namespace FubuMVC.Core.UI.Scripts
{
    public interface IScriptObject
    {
        string Name { get; }
        bool Matches(string key);
        void AddAlias(string alias);

        IEnumerable<IScript> AllScripts(ScriptGraph graph);
    }
}