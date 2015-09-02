using System;
using System.IO;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Tests.Runtime.Files
{
    public class StubFubuFile : IFubuFile
    {
        private readonly DateTime _lastModified;

        public StubFubuFile(string path, DateTime lastModified)
        {
            Path = RelativePath = path;
            _lastModified = lastModified;
        }

        public StubFubuFile ChangedVersion()
        {
            return new StubFubuFile(Path, LastModified().AddSeconds(5));
        }

        public string Path { get; private set; }
        public string RelativePath { get; set; }
        public string ReadContents()
        {
            throw new NotImplementedException();
        }

        public void ReadContents(Action<Stream> action)
        {
            throw new NotImplementedException();
        }

        public void ReadLines(Action<string> read)
        {
            throw new NotImplementedException();
        }

        public long Length()
        {
            throw new NotImplementedException();
        }

        public string Etag()
        {
            throw new NotImplementedException();
        }

        public DateTime LastModified()
        {
            return _lastModified;
        }

        protected bool Equals(StubFubuFile other)
        {
            return string.Equals(Path, other.Path);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StubFubuFile) obj);
        }

        public override int GetHashCode()
        {
            return (Path != null ? Path.GetHashCode() : 0);
        }
    }
}