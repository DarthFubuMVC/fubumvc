using System.Collections.Generic;

namespace FubuMVC.Core.UI.Scripts
{
    public interface IScriptObject : IEnumerable<IScript>
    {
        string Name { get; }
        bool Matches(string key);
        void AddAlias(string alias);

        IEnumerable<IScript> AllScripts();
        IEnumerable<IScriptObject> Dependencies();
        void AddDependency(IScriptObject scriptObject);
    }
}