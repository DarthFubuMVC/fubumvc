using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace FubuMVC.Core.Assets.Files
{
    public class AssetPath : ResourcePath
    {
        public AssetPath(IEnumerable<string> pathParts) : this(pathParts.Join("/"))
        {
        }

        public AssetPath(string path) : base(path)
        {
            if (path.Contains(":"))
            {
                var parts = path.Split(':');
                if (parts.Length > 2)
                {
                    throw new AssetPathException(path);
                }

                Package = parts.First();
                path = parts.Last();
            }

            readPath(path);
        }

        public AssetPath(string package, string name, AssetFolder folder) : base(name)
        {
            if (package == null) throw new ArgumentNullException("package");
            if (name == null) throw new ArgumentNullException("name");

            Name = name;
            Package = package;
            Folder = folder;
        }

        public AssetPath(string path, AssetFolder folder) : base(path)
        {
            Folder = folder;
        }

        public string ToFullName()
        {
            var name = "";

            Package.IfNotNull(x => name += x + ":");
            if (Folder != null) name += Folder.Name + "/";
            
            name += Name;

            return name;
        }

        [ResourceHash]
        public string ResourceHash { get; set; }

        public string Name { get; private set; }
        public string Package { get; private set; }
        public AssetFolder Folder { get; private set; }

        private void readPath(string path)
        {
            if (!path.Contains("/"))
            {
                Name = path;
                return;
            }

            foreach (AssetFolder type in AssetFolder.AllFolders())
            {
                var prefix = type + "/";

                if (path.StartsWith(prefix))
                {
                    Folder = type;
                    Name = path.Substring(prefix.Length);
                    return;
                }
            }

            Name = path;
        }

        // TODO -- this smells to high heaven and it's going to be brittle
        public bool IsBinary()
        {
            var mimeType = MimeType.MimeTypeByFileName(Name);

            if (mimeType != null)
            {
                return mimeType != MimeType.Css && mimeType != MimeType.Javascript;
            }

            return Folder.Equals(AssetFolder.images) || Folder.Equals(AssetFolder.fonts);
        }

        public bool Equals(AssetPath other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.Package, Package) && other.Folder.Equals(Folder);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AssetPath)) return false;
            return Equals((AssetPath) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ (Package != null ? Package.GetHashCode() : 0);
                result = (result*397) ^ (Folder != null ? Folder.GetHashCode() : 0);
                return result;
            }
        }
    }
}