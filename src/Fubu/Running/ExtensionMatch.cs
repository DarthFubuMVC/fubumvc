using System.IO;
using FubuCore;

namespace Fubu.Running
{
    public class ExtensionMatch : IFileMatch
    {
        private readonly FileChangeCategory _category;
        private readonly string _extension;

        public ExtensionMatch(FileChangeCategory category, string extension)
        {
            _category = category;
            _extension = Path.GetExtension(extension);
        }

        public FileChangeCategory Category
        {
            get { return _category; }
        }

        public string Extension
        {
            get { return _extension; }
        }

        public bool Matches(string file)
        {
            return Path.GetExtension(file).EqualsIgnoreCase(_extension);
        }

        protected bool Equals(ExtensionMatch other)
        {
            return _category == other._category && string.Equals(_extension, other._extension);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExtensionMatch) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) _category*397) ^ (_extension != null ? _extension.GetHashCode() : 0);
            }
        }
    }
}