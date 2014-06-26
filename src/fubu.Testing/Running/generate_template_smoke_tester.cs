using System;
using System.Linq;
using Fubu.Running;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing.Running
{
    [TestFixture, Explicit("Test hangs, but the real functionality works")]
    public class generate_template_smoke_tester
    {
        [Test]
        public void regenerate_the_templates()
        {
            var applicationPath =
                AppDomain.CurrentDomain.BaseDirectory
                .ParentDirectory().ParentDirectory()
                .AppendPath("FubuApp").ToFullPath();

            var templatePath = applicationPath.AppendPath("_templates");

            var files = new FileSystem();
            files.CleanDirectory(templatePath);

            var request = new ApplicationRequest
            {
                TemplatesFlag = true,
                DirectoryFlag = applicationPath
            };

            new RunCommand().Execute(request).ShouldBeTrue();

            files.FindFiles(templatePath, FileSet.Deep("*.htm")).Select(x => x.PathRelativeTo(applicationPath).Replace('\\', '/'))
                .OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("_templates/en-US/Blue.htm", "_templates/en-US/Green.htm", "_templates/en-US/Red.htm");
        }
    }
}