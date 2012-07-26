using FubuMVC.Core.Runtime.Files;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Runtime.Files
{
    [TestFixture]
    public class FubuApplicationFilesSmokeTester
    {
        private FubuApplicationFiles theFiles;

        [SetUp]
        public void SetUp()
        {
            theFiles = new FubuApplicationFiles();
        }

        [Test]
        public void find_file()
        {
            theFiles.Find("runtime/files/data/a.txt").ReadContents()
                .Trim().ShouldEqual("some text from a.txt");
        }

        [Test]
        public void find_file_canonicizes_paths()
        {
            theFiles.Find("runtime\\files\\data\\A.txt").ReadContents()
                .Trim().ShouldEqual("some text from a.txt");
        }
    }
}