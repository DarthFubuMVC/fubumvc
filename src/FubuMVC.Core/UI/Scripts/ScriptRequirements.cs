using System;
using System.Collections.Generic;
using FubuMVC.Core.Content;

namespace FubuMVC.Core.UI.Scripts
{
    public class ScriptRequirements
    {
        private readonly IContentFolderService _folders;
        private readonly List<string> _configuredScripts = new List<string>();
        private readonly List<string> _pageScripts = new List<string>();

        public ScriptRequirements(IContentFolderService folders)
        {
            _folders = folders;
        }

        public void ConfiguredScript(string name)
        {
            _configuredScripts.Fill(name);
        }

        public void PageScript(string name)
        {
            _pageScripts.Fill(name);
        }

        public IEnumerable<string> AllConfiguredScriptNames()
        {
            return _configuredScripts;
        }

        public IEnumerable<string> AllPageScriptNames()
        {
            return _pageScripts;
        }

        public void UseFileIfExists(string name)
        {
            if (_folders.FileExists(ContentType.scripts, name))
            {
                ConfiguredScript(name);
            }
        }
    }


   
}