using System;
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
        public static readonly string Overrides = "overrides";

        private readonly string _name;
        private MimeType _mimeType;

        public AssetFile(string name)
        {
            if (name.StartsWith(Overrides))
            {
                Override = true;
                name = name.Substring(Overrides.Length + 1);
            }

            _name = name;

            var mimeType = MimeType.MimeTypeByFileName(Name);
            if (mimeType != null)
            {
                _mimeType = mimeType;
            }


        }

        public string ContentFolder()
        {
            if (!_name.Contains('/'))
            {
                return null;
            }

            return _name.Split('/').Reverse().Skip(1).Reverse().Join("/");
        }

        public AssetFile(string name, AssetFolder folder) : this(name)
        {
            Folder = folder;

            if (_mimeType == null && folder != null)
            {
                _mimeType = folder.DefaultMimetype();
            }
        }



        public string FullPath { get; set; }
        public bool Override { get; set; }


        private AssetFolder _folder;
        public AssetFolder Folder
        {
            get
            {
                if (_folder == null && _mimeType != null)
                {
                    _folder = AssetFolder.FolderFor(_mimeType);
                }
                
                
                return _folder;
            }
            set { _folder = value; }
        }

        public string Name
        {
            get { return _name; }
        }

        public MimeType MimeType
        {
            get
            {
                if (_mimeType == null && _folder != null)
                {
                    _mimeType = Folder.DefaultMimetype();
                }

                return _mimeType;
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

        public bool Equals(AssetFile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (other.Folder == Folder) && Equals(other.Name, Name) && Equals(other.FullPath, FullPath);
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
                var result = (Folder != null ? Folder.GetHashCode() : 0);
                result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (FullPath != null ? FullPath.GetHashCode() : 0);
                return result;
            }
        }


        public IEnumerable<string> AllExtensions()
        {
            return Name.Split('.').Skip(1).Select(x => "." + x);
        }

        public bool MatchesFullPath(string path)
        {
            return string.Equals(FullPath, path, StringComparison.InvariantCultureIgnoreCase);
        }

        public string LibraryName()
        {
            return Name.Split('/').Last();
        }
    }
}