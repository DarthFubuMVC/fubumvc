using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public class MissingAssetTagSubject : IAssetTagSubject
    {
        public MissingAssetTagSubject(string name)
        {
            Name = name;

            MimeType = MimeType.MimeTypeByFileName(name);
        }

        public string Name { get; private set; }

        public AssetFolder Folder
        {
            get { return null; }
        }

        public MimeType MimeType { get; private set; }
    }
}