using System;
using System.Globalization;
using System.IO;
using FubuCore;

namespace FubuMVC.Core.Runtime.Files
{
    public class FubuFile : IFubuFile
    {
        public FubuFile(string path, string provenance)
        {
            Path = path;
            Provenance = provenance;
        }

        public string Path { get; private set; }
        public string Provenance { get; private set; }
        public string RelativePath { get; set; }
        public string ProvenancePath { get; set; }

        public string ReadContents()
        {
            return new FileSystem().ReadStringFromFile(Path);
        }

        public void ReadContents(Action<Stream> action)
        {
            using (var stream = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                action(stream);
            }
        }

        public void ReadLines(Action<string> read)
        {
            new FileSystem().ReadTextFile(Path, read);
        }

        public long Length()
        {
            return new FileInfo(Path).Length;
        }

        public string Etag()
        {
            var length = Length();
            var lastModified = LastModified();

            var hash = lastModified.ToFileTimeUtc() ^ length;

            return Convert.ToString(hash, 16);
        }

        public DateTime LastModified()
        {
            var last = new FileInfo(Path).LastWriteTimeUtc;
            return new DateTime(last.Year, last.Month, last.Day, last.Hour, last.Minute, last.Second, last.Kind);
        }

        public bool Equals(FubuFile other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Path, Path) && Equals(other.Provenance, Provenance);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FubuFile)) return false;
            return Equals((FubuFile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Path != null ? Path.GetHashCode() : 0)*397) ^ (Provenance != null ? Provenance.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Path: {0}, Provenance: {1}", Path, Provenance);
        }

        public static IFubuFile Load(string relativePath)
        {
            return new FubuApplicationFiles().Find(relativePath);
        }
    }
}