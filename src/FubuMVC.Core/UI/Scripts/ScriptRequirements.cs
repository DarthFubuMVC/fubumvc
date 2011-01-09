using System;
using System.Collections.Generic;
using FubuMVC.Core.Content;

namespace FubuMVC.Core.UI.Scripts
{
    public class ScriptRequirements
    {
        private readonly IContentFolderService _folders;
        private readonly List<string> _requirements = new List<string>();

        public ScriptRequirements(IContentFolderService folders)
        {
            _folders = folders;
        }

        public void Require(string name)
        {
            _requirements.Fill(name);
        }

        public IEnumerable<string> AllScriptNames()
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
    }


   
}