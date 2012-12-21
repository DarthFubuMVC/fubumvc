using System;
using System.Collections.Generic;
using System.IO;
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
        public void get_application_path_delegates_to_fubumvc_package_facility()
        {
            theFiles.GetApplicationPath().ShouldEqual(FubuMvcPackageFacility.GetApplicationPath());
        }
    }
}