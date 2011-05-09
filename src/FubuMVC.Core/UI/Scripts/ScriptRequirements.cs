using System.Collections.Generic;
using FubuMVC.Core.Content;
using System.Linq;

namespace FubuMVC.Core.UI.Scripts
{
    public class ScriptRequirements
    {
        private readonly IContentFolderService _folders;
        private readonly ScriptGraph _scriptGraph;
        private readonly List<string> _requirements = new List<string>();
        private readonly List<string> _rendered = new List<string>();

        public ScriptRequirements(IContentFolderService folders, ScriptGraph scriptGraph)
        {
            _folders = folders;
            _scriptGraph = scriptGraph;
        }

        public void Require(string name)
        {
            _requirements.Fill(name);
        }

        public IEnumerable<string> RequestedScripts()
        {
            return _requirements;
        }

        public void UseFileIfExists(string name)
        {
            if (_folders.FileExists(ContentType.scripts, name))
            {
                Require(name);
            }
        }

        /// <summary>
        /// Returns a list of scripts that have been Required, and any of their dependencies.
        /// </summary>
        /// <remarks>Can be called multiple times within an HTTP request, and will not return any script more than once.</remarks>
        /// <returns></returns>
        public IEnumerable<string> GetScriptsToRender()
        {
            var requiredScripts = _scriptGraph.GetScripts(_requirements).Select(x => x.Name).Except(_rendered).ToList();
            _rendered.AddRange(requiredScripts);
            return requiredScripts;
        }
    }


   
}