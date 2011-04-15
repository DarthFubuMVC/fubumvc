using System.IO;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace Bottles.Tests
{
    [TestFixture]
    public class FileSystemTester : InteractionContext<FileSystem>
    {
        protected override void beforeEach()
        {
            if(Directory.Exists(@".\home"))
                Directory.Delete(@".\home",true);

            Directory.CreateDirectory(@".\home");

            File.Create(@".\home\bob.txt").Close();
            File.Create(@".\home\mary.txt").Close();
        }

        [Test]
        public void should_work_with_relative_paths()
        {
            if (Directory.Exists(@".\rome"))
                Directory.Delete(@".\rome", true);

            ClassUnderTest.Copy(@".\home", @"rome");
        }
    }
}