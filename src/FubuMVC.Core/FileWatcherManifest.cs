namespace FubuMVC.Core
{
    public class FileWatcherManifest
    {
        public string ConfigurationFile { get; set; }
        public string[] ContentMatches { get; set; }

        public string PublicAssetFolder { get; set; }

        public string[] AssetExtensions { get; set; }

        public string[] TemplateFiles { get; set; }
    }
}