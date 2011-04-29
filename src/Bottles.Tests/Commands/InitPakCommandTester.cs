using Bottles.Commands;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests.Commands
{
    [TestFixture]
    public class InitPakCommandTester
    {
        [Test]
        public void NAME()
        {
            var fs = new FileSystem();
            fs.DeleteDirectory("initpath");


            var input = new InitPakInput()
                        {
                            Name="inittest",
                            Path = "initpath"
                        };
            
            var cmd = new InitPakCommand();

            cmd.Execute(input);

            fs.FileExists("initpath",PackageManifest.FILE).ShouldBeTrue();

            //check entries in alias?

            var pm = fs.LoadPackageManifestFrom("initpath");

            pm.Name.ShouldEqual("inittest");

            //clean up 
            fs.DeleteDirectory("initpath");
        }
    }
}