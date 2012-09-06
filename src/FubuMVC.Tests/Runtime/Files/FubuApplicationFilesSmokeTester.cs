using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Packaging;
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

        [Test]
        public void is_not_under_exploded_bottle_folder_negative()
        {
            new[] { FubuMvcPackageFacility.FubuContentFolder, FubuMvcPackageFacility.FubuPackagesFolder}.Each(folder =>
            {
                var directory = "runtime/{0}/p1".ToFormat(folder);
                var fubuFile = new FubuFile("{0}/a.txt".ToFormat(directory), "p1");
                fubuFile.RelativePath = fubuFile.Path.PathRelativeTo(directory);

                FubuApplicationFiles.IsNotUnderExplodedBottleFolder(fubuFile).ShouldBeFalse();
            });
        }

        [Test]
        public void is_not_under_exploded_bottle_folder_positive()
        {
            new[] { "some/content", "custom/packages", "files" }.Each(folder =>
            {
                var directory = "runtime/{0}".ToFormat(folder);
                var fubuFile = new FubuFile("{0}/b.txt".ToFormat(directory), "app");
                fubuFile.RelativePath = fubuFile.Path.PathRelativeTo(directory);

                FubuApplicationFiles.IsNotUnderExplodedBottleFolder(fubuFile).ShouldBeTrue();
            });
        }

        [Test]
        public void get_application_path_delegates_to_fubumvc_package_facility()
        {
            theFiles.GetApplicationPath().ShouldEqual(FubuMvcPackageFacility.GetApplicationPath());
        }
    }
}