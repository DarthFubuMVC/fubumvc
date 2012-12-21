using System;
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
            theFiles.AssertHasFile("Runtime/Files/Data/a.txt");

            IFubuFile fubuFile = theFiles.Find("Runtime/Files/Data/a.txt");
            fubuFile.ShouldNotBeNull();

            fubuFile.ReadContents()
                .Trim().ShouldEqual("some text from a.txt");
        }

        [Test]
        public void find_file_canonicizes_paths()
        {
            theFiles.AssertHasFile("Runtime\\Files\\Data\\a.txt");

            theFiles.Find("Runtime\\Files\\Data\\a.txt").ReadContents()
                .Trim().ShouldEqual("some text from a.txt");
        }

        [Test]
        public void is_not_under_exploded_bottle_folder_negative()
        {
            new[] { FubuMvcPackageFacility.FubuContentFolder, FubuMvcPackageFacility.FubuPackagesFolder}.Each(folder =>
            {
                var directory = "Runtime/{0}/p1".ToFormat(folder);
                var fubuFile = new FubuFile("{0}/a.txt".ToFormat(directory), "p1");
                fubuFile.RelativePath = fubuFile.Path.PathRelativeTo(directory);


                if (FubuApplicationFiles.IsNotUnderExplodedBottleFolder(fubuFile))
                {
                    Assert.Fail(fubuFile.RelativePath + " was considered to be under the exploded bottle folder - " + directory + fubuFile.Path);
                }
            });
        }

        [Test]
        public void is_not_under_exploded_bottle_folder_positive()
        {
            new[] { "some/content", "custom/packages", "files" }.Each(folder =>
            {
                var directory = "Runtime/{0}".ToFormat(folder);
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