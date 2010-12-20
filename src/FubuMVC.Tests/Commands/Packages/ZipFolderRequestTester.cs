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

        private IEnumerable<StubZipEntry> theResultingZipEntries()
        {
            var zipFile = new StubZipFile();
            theRequest.WriteToZipFile(zipFile);

            return zipFile.AllZipEntries;
        }

        // Not really worried about filters here.  Just enough to prove that it is
        // using the filter
        [Test]
        public void use_single_include_filter()
        {
            writeFile("zip\\a.txt");
            writeFile("zip\\f1\\b.txt");
            writeFile("zip\\f1\\f2\\c.txt");
            writeFile("zip\\a.xml");
            writeFile("zip\\b.xml");

            theRequest.FileSet.Include = "*.txt";

            theResultingZipEntries().ShouldHaveTheSameElementsAs(
                new StubZipEntry("zip\\a.txt", string.Empty),
                new StubZipEntry("zip\\f1\\b.txt", "f1"),
                new StubZipEntry("zip\\f1\\f2\\c.txt", "f1\\f2")
                );
        }

        [Test]
        public void use_single_exclude_filter()
        {
            writeFile("zip\\a.txt");
            writeFile("zip\\f1\\b.txt");
            writeFile("zip\\f1\\f2\\c.txt");
            writeFile("zip\\a.xml");
            writeFile("zip\\b.xml");

            theRequest.FileSet.Exclude = "*.xml";

            theResultingZipEntries().ShouldHaveTheSameElementsAs(
                new StubZipEntry("zip\\a.txt", string.Empty),
                new StubZipEntry("zip\\f1\\b.txt", "f1"),
                new StubZipEntry("zip\\f1\\f2\\c.txt", "f1\\f2")
                );
        }

        [Test]
        public void use_overlapping_include_and_exclude_filters()
        {
            writeFile("zip\\a.txt");
            writeFile("zip\\f1\\b.txt");
            writeFile("zip\\f1\\f2\\c.txt");
            writeFile("zip\\a.xml");
            writeFile("zip\\b.xml");

            theRequest.FileSet.Include = "a.*";
            theRequest.FileSet.Exclude = "*.xml";

            theResultingZipEntries().ShouldHaveTheSameElementsAs(
                new StubZipEntry("zip\\a.txt", string.Empty)
                );
        }
    }

    public class StubZipFile : IZipFile
    {
        public readonly IList<StubZipEntry> AllZipEntries = new List<StubZipEntry>();
        public readonly IList<ZipFolderRequest> ZipRequests = new List<ZipFolderRequest>();

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
            ZipRequests.Add(request);
        }
    }

    public class StubZipEntry
    {
        public StubZipEntry()
        {
        }

        public StubZipEntry(string file, string zipFolder)
        {
            File = file.ToFullPath();
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