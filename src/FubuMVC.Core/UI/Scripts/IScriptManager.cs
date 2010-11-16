using System.Collections.Generic;

namespace FubuMVC.UI.Scripts
{
    public interface IScriptManager
    {
        void RegisterInclude(string path);
        IEnumerable<Script> RegisteredScripts();
    }
}