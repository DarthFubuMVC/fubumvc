using Bottles.Commands;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests.Commands
{
    [TestFixture]
    public class InitPakCommandTester 
    {
        IFileSystem fs = new FileSystem();
        private string thePath = "initpath";
        private string pakName = "init-test";
        private InitPakInput theInput;

        [SetUp]
        public void BeforeEach()
        {
            fs.DeleteDirectory(thePath);
            theInput = new InitPakInput
                       {
                           Name = pakName,
                           Path = thePath
                       };
        }

        string checkForAlias(string alias)
        {
            var a = new FileSystem()
                  .LoadFromFile<AliasRegistry>(AliasRegistry.ALIAS_FILE);

            return a.AliasFor(alias).Folder;
        }

        void execute()
        {
            
            var cmd = new InitPakCommand();
            cmd.Execute(theInput);
        }

        [Test]
        public void the_pak_should_have_been_created()
        {
            execute();

            fs.FileExists(thePath, PackageManifest.FILE).ShouldBeTrue();

            checkForAlias(pakName).ShouldEqual(thePath);

            var pm = fs.LoadPackageManifestFrom(thePath);

            pm.Name.ShouldEqual(pakName);
        }

        [Test]
        public void the_pak_should_have_been_created_with_alias()
        {
            var theAlias = "blue";
            theInput.AliasFlag = theAlias;

            execute();

            fs.FileExists(thePath, PackageManifest.FILE).ShouldBeTrue();

            checkForAlias(theAlias).ShouldEqual(thePath);

            var pm = fs.LoadPackageManifestFrom(thePath);

            pm.Name.ShouldEqual(pakName);
        }
    }
}