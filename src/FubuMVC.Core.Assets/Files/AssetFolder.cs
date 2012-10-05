using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Core.Assets.Files
{
    public class AssetFolder
    {
        private static readonly Cache<string, AssetFolder> _folders = new Cache<string, AssetFolder>(folder => new AssetFolder(folder, folder));
        private static readonly Cache<MimeType, AssetFolder> _byMimeType; 

        public string Folder { get; set; }
        private readonly string _name;
        public static readonly AssetFolder images = For("images").Mimetype(MimeType.Gif, MimeType.Jpg, MimeType.Bmp, MimeType.Png);
        public static readonly AssetFolder scripts = For("scripts").Mimetype(MimeType.Javascript);
        public static readonly AssetFolder styles = For("styles").Mimetype(MimeType.Css);
        public static readonly AssetFolder fonts = For("fonts").Mimetype(MimeType.TrueTypeFont);

        static AssetFolder()
        {
            _byMimeType = new Cache<MimeType, AssetFolder>(mimeType => {
                return _folders.FirstOrDefault(x => x._mimeTypes.Contains(mimeType));
            });
        }

        public static IEnumerable<AssetFolder> AllFolders()
        {
            return _folders;
        } 

        public static AssetFolder For(string name, string folderName = null)
        {
            var folder = _folders[name];
            folder.Folder = folderName ?? name;

            return folder;
        }

        private AssetFolder(string name, string folder)
        {
            Folder = folder;
            _name = name;
        }

        

        public MimeType DefaultMimetype()
        {
            return _mimeTypes.FirstOrDefault();
        }

        private readonly List<MimeType> _mimeTypes = new List<MimeType>(); 
        public AssetFolder Mimetype(params MimeType[] mimeType)
        {
            _mimeTypes.AddRange(mimeType);
            return this;
        }

        public string Name
        {
            get { return _name; }
        }

        protected bool Equals(AssetFolder other)
        {
            return string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AssetFolder) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return Folder;
        }



        public static AssetFolder FolderFor(MimeType mimeType)
        {
            return _byMimeType[mimeType];
        }
    }
}