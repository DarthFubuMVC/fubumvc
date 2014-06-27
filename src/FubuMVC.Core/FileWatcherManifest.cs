using System;

namespace FubuMVC.Core
{
    public class FileWatcherManifest
    {
        public string ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        public string[] ContentMatches = new string[0];

        public string PublicAssetFolder = string.Empty;

        public string[] AssetExtensions = new string[0];
    }
}