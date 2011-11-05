using System;
using Bottles;
using FubuCore;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using Rhino.Mocks;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class ContentOnlyPackageInfoTester
    {
        private ContentOnlyPackageInfo thePackage;

        [SetUp]
        public void SetUp()
        {
            new FileSystem().CreateDirectory("content-package");
            thePackage = new ContentOnlyPackageInfo("content-package");
        }

        [Test]
        public void no_dependencies()
        {
            thePackage.GetDependencies().Any().ShouldBeFalse();
        }

        [Test]
        public void does_find_the_web_content_folder()
        {
            var action = MockRepository.GenerateMock<Action<string>>();

            thePackage.ForFolder("Data", action);
            action.AssertWasNotCalled(x => x.Invoke("content-package"));

            thePackage.ForFolder(BottleFiles.WebContentFolder, action);
            action.AssertWasCalled(x => x.Invoke("content-package"));
        }
    }
}