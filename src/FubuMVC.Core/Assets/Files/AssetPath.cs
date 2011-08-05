using System;
using System.Linq;

namespace FubuMVC.Core.Assets.Files
{
    public class AssetPath
    {
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

        private void readPath(string path)
        {
            if (!path.Contains("/"))
            {
                Name = path;
                return;
            }

            foreach (AssetType type in Enum.GetValues(typeof(AssetType)))
            {
                var prefix = type.ToString() + "/";

                if (path.StartsWith(prefix))
                {
                    Type = type;
                    Name = path.Substring(prefix.Length);
                    return;
                }
            }

            Name = path;
        }

        public string Name { get; private set; }
        public string Package { get; private set; }
        public AssetType? Type { get; private set; }
    }
}