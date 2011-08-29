using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.Assets.Files
{
    public class AssetPath
    {
        public AssetPath(IEnumerable<string> pathParts) : this(pathParts.Join("/"))
        {
        }

        public AssetPath(string path)
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

        public AssetPath(string package, string name, AssetFolder? folder)
        {
            if (package == null) throw new ArgumentNullException("package");
            if (name == null) throw new ArgumentNullException("name");

            Name = name;
            Package = package;
            Folder = folder;
        }

        public string Name { get; private set; }
        public string Package { get; private set; }
        public AssetFolder? Folder { get; private set; }

        private void readPath(string path)
        {
            if (!path.Contains("/"))
            {
                Name = path;
                return;
            }

            foreach (AssetFolder type in Enum.GetValues(typeof (AssetFolder)))
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
    }
}