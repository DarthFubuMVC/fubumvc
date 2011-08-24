using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Files
{
    /// <summary>
    ///   Make AssetFile dumb
    ///   Forget CDN for now
    ///   Forget LastChanged
    /// </summary>
    public class AssetFile : IAssetTagSubject
    {
        public AssetFile()
        {
        }

        public AssetFile(string name)
        {
            Name = name;
        }

        public AssetFile(string name, AssetFolder? folder)
        {
            Name = name;
            Folder = folder;
        }

        public string FullPath { get; set; }
        //public DateTime LastChanged { get; set; }
        public bool Override { get; set; }
        public AssetFolder? Folder { get; set; }

        public string Name { get; set; }

        public MimeType MimeType
        {
            get
            {
                var mimeType = MimeType.DetermineMimeTypeFromName(Name);
                if (mimeType != null) return mimeType;

                switch (Folder)
                {
                    case AssetFolder.scripts:
                        return MimeType.Javascript;

                    case AssetFolder.styles:
                        return MimeType.Css;

                    default:
                        throw new UnknownExtensionException(Extension());
                }
            }
        }

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


        // TODO -- just pull MimeType live

        public bool Equals(AssetFile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Folder.Equals(Folder) && Equals(other.Name, Name) && Equals(other.FullPath, FullPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AssetFile)) return false;
            return Equals((AssetFile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (Folder.HasValue ? Folder.Value.GetHashCode() : 0);
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (FullPath != null ? FullPath.GetHashCode() : 0);
                return result;
            }
        }


        public IEnumerable<string> AllExtensions()
        {
            return Name.Split('.').Skip(1).Select(x => "." + x);
        }
    }
}