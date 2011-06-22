using System.Collections.Generic;

namespace FubuMVC.Core.UI.Scripts
{
    public interface IScriptObject : IEnumerable<IScript>
    {
        string Name { get; }
        string FallbackName { get; }
        string WindowVariableName { get; }
        bool HasFallback { get; }
        bool Matches(string key);
        void AddAlias(string alias);
        void AddFallback(string fallbackName, string windowVariableName);

        IEnumerable<IScript> AllScripts();
        IEnumerable<IScriptObject> Dependencies();
        void AddDependency(IScriptObject scriptObject);
    }
}