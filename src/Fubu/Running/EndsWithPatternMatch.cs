using System;
using System.IO;

namespace Fubu.Running
{
    public class EndsWithPatternMatch : IFileMatch
    {
        private readonly FileChangeCategory _category;
        private readonly string _match;
        private string _suffix;

        public EndsWithPatternMatch(FileChangeCategory category, string match)
        {
            if (!match.StartsWith("*"))
            {
                throw new ArgumentOutOfRangeException("match", "Pattern must start with an '*'");
            }
            
            _category = category;
            _match = match;

            _suffix = _match.TrimStart('*');
        }

        public string Match
        {
            get { return _match; }
        }

        protected bool Equals(EndsWithPatternMatch other)
        {
            return _category == other._category && string.Equals(_match, other._match);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EndsWithPatternMatch) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) _category*397) ^ (_match != null ? _match.GetHashCode() : 0);
            }
        }

        public bool Matches(string file)
        {
            return Path.GetFileName(file).EndsWith(_suffix, StringComparison.OrdinalIgnoreCase);
        }

        public FileChangeCategory Category
        {
            get { return _category; }
        }
    }
}