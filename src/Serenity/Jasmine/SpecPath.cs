using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Files;

namespace Serenity.Jasmine
{
    public class SpecPath
    {
        private readonly IList<string> _parts;

        public SpecPath(IList<string> parts)
        {
            _parts = parts;
        }

        public SpecPath(string packageName, string assetName)
        {
            _parts = new List<string>{
                packageName
            };

            _parts.AddRange(assetName.Split('/'));
        }

        public SpecPath(string fullPath)
        {
            _parts = new List<string>();

            var path = new AssetPath(fullPath);
            if (path.Package != null) _parts.Add(path.Package);
            _parts.AddRange(path.Name.Split('/'));
        }

        private string fullname
        {
            get
            {
                return _parts.Join("/");
            }
        }

        public bool Equals(SpecPath other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.fullname, fullname);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SpecPath)) return false;
            return Equals((SpecPath) obj);
        }

        public override int GetHashCode()
        {
            return (fullname != null ? fullname.GetHashCode() : 0);
        }

        public string TopFolder
        {
            get { return _parts.FirstOrDefault(); }
        }

        public IList<string> Parts
        {
            get { return _parts; }
        }

        public SpecPath ChildPath()
        {
            return new SpecPath(_parts.Skip(1).ToList());
        }

    }
}