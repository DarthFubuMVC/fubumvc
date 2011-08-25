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
        private readonly string _name;
        private MimeType _mimeType;

        public AssetFile(string name)
        {
            _name = name;

            var mimeType = MimeType.DetermineMimeTypeFromName(Name);
            if (mimeType != null)
            {
                _mimeType = mimeType;
            }


        }

        public AssetFile(string name, AssetFolder? folder) : this(name)
        {
            Folder = folder;

            if (_mimeType == null && folder.HasValue)
            {
                _mimeType = MimeType.ForFolder(folder.Value);
            }
        }



        public string FullPath { get; set; }
        public bool Override { get; set; }


        private AssetFolder? _folder;
        public AssetFolder? Folder
        {
            get
            {
                if (!_folder.HasValue && _mimeType != null)
                {
                    _folder = _mimeType.Folder();
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
                if (_mimeType == null && _folder.HasValue)
                {
                    _mimeType = MimeType.ForFolder(_folder.Value);
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