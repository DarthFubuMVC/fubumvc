using System.IO;
using FubuCore;

namespace Fubu.Running
{
    public class ExactFileMatch : IFileMatch
    {
        private readonly FileChangeCategory _category;
        private readonly string _file;

        public ExactFileMatch(FileChangeCategory category, string file)
        {
            _category = category;
            _file = file;
        }

        public bool Matches(string file)
        {
            return Path.GetFileName(file).EqualsIgnoreCase(_file);
        }

        public FileChangeCategory Category
        {
            get { return _category; }
        }

        protected bool Equals(ExactFileMatch other)
        {
            return _category == other._category && string.Equals(_file, other._file);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExactFileMatch) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) _category*397) ^ (_file != null ? _file.GetHashCode() : 0);
            }
        }
    }
}