using System;
using System.IO;
using FubuMVC.Core.Content;

namespace FubuMVC.Core.Assets.Files
{
    /// <summary>
    ///  Make AssetFile dumb
    /// Forget CDN for now
    /// Forget LastChanged
    /// </summary>
    public class AssetFile 
    {
        public AssetFolder? Folder { get; set; }

        public string Name { get; set; }
        public string FullPath { get; set; }
        //public DateTime LastChanged { get; set; }
        public bool Override { get; set; }

        public override string ToString()
        {
            var description = string.Format("Asset: {0} at {1}", Name, FullPath);
            if (Override)
            {
                description = description + " (Override)";
            }

            return description;
        }

        public string Extension()
        {
            return Path.GetExtension(Name);
        }

        public string MimeType { get; private set; }

        public void DetermineMimetype(IMimeTypeProvider provider)
        {
            MimeType = provider.For(Extension());
        }
    }
}