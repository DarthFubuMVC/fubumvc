using System;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Templates;

namespace FubuMVC.Core
{
    public class FileWatcherManifest
    {
        public static FileWatcherManifest Build(AssetSettings settings, TemplateGraph templates)
        {
            throw new NotImplementedException();
        }

        public string ConfigurationFile =  AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        public string[] ContentMatches = new string[0];

        public string PublicAssetFolder = string.Empty;

        public string[] AssetExtensions  = new string[0];

        public string[] TemplateFiles = new string[0];
    }
}