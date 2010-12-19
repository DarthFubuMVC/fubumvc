using System;
using System.Collections.Generic;
using System.IO;
using Fubu.Packages;
using FubuCore;
using FubuMVC.Core.Packaging;
using NUnit.Framework;

namespace FubuMVC.Tests.Commands.Packages
{
    [TestFixture]
    public class ZipFolderRequestTester
    {
        private ZipFolderRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            if (Directory.Exists("zip"))
            {
                Directory.Delete("zip", true);
            }

            Directory.CreateDirectory("zip");

            theRequest = new ZipFolderRequest(){
                FileSet = new FileSet(),
                RootDirectory = Path.GetFullPath("zip"),
                ZipDirectory = ""
            };
        }


        private void writeFile(string name)
        {
            var directory = Path.GetDirectoryName(name);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(name))
            {
                File.Delete(name);
            }

            File.WriteAllText(name, "");
        }

        //[Test]
        //public void use_single_include_filter_and_flat_folder()
        //{
        //    writeFile("zip/a.txt");
        //    writeFile("zip/b.txt");
        //    writeFile("zip/c.txt");
        //    writeFile("zip/a.xml");
        //    writeFile("zip/b.xml");

        //    theRequest.FileSet.Include = ""
        //}
    }

    public class StubZipFile : IZipFile
    {
        public readonly IList<StubZipEntry> AllZipEntries = new List<StubZipEntry>();

        public void AddFile(string fileName)
        {
            AllZipEntries.Add(new StubZipEntry(fileName, string.Empty));
        }

        public void AddFile(string fileName, string zipFolder)
        {
            AllZipEntries.Add(new StubZipEntry(fileName, zipFolder));
        }

        public Guid AddVersion()
        {
            throw new NotImplementedException();
        }

        public void AddFiles(ZipFolderRequest request)
        {
            request.WriteToZipFile(this);
        }
    }

    public class StubZipEntry
    {
        public StubZipEntry()
        {
        }

        public StubZipEntry(string file, string zipFolder)
        {
            File = file;
            ZipFolder = zipFolder;
        }

        public bool Equals(StubZipEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.File, File) && Equals(other.ZipFolder, ZipFolder);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (StubZipEntry)) return false;
            return Equals((StubZipEntry) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((File != null ? File.GetHashCode() : 0)*397) ^ (ZipFolder != null ? ZipFolder.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("File: {0}, ZipFolder: {1}", File, ZipFolder);
        }

        public string File { get; set; }
        public string ZipFolder { get; set; }
    }
}