using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Files
{
    [TestFixture]
    public class AssetPathTester
    {
        [Test]
        public void mangled_path_throws_exception()
        {
            Exception<AssetPathException>.ShouldBeThrownBy(() =>
            {
                new AssetPath("pak1:pak2:jquery.js");
            });
        }

        [Test]
        public void simple_path_with_no_package_or_type()
        {
            var path = new AssetPath("jquery.js");
            path.Folder.ShouldBeNull();
            path.Name.ShouldEqual("jquery.js");
            path.Package.ShouldBeNull();
        }

        [Test]
        public void path_with_type_specified_but_no_package()
        {
            var path = new AssetPath("scripts/jquery.js");
            path.Folder.ShouldEqual(AssetFolder.scripts);
            path.Name.ShouldEqual("jquery.js");
            path.Package.ShouldBeNull();
        }

        [Test]
        public void path_with_package_and_type_specified()
        {
            var path = new AssetPath("pak1:scripts/jquery.js");
            path.Folder.ShouldEqual(AssetFolder.scripts);
            path.Name.ShouldEqual("jquery.js");
            path.Package.ShouldEqual("pak1"); 
        }

        [Test]
        public void path_with_package_but_no_type_specified()
        {
            var path = new AssetPath("pak1:jquery.js");
            path.Folder.ShouldBeNull();
            path.Name.ShouldEqual("jquery.js");
            path.Package.ShouldEqual("pak1"); 
        }

        [Test]
        public void deep_path()
        {
            var path = new AssetPath("folder/jquery.js");
            path.Folder.ShouldBeNull();
            path.Name.ShouldEqual("folder/jquery.js");
            path.Package.ShouldBeNull(); 
        }

        [Test]
        public void deep_path_with_type()
        {
            var path = new AssetPath("scripts/folder/jquery.js");
            path.Folder.ShouldEqual(AssetFolder.scripts);
            path.Name.ShouldEqual("folder/jquery.js");
            path.Package.ShouldBeNull();
        }

        [Test]
        public void deep_path_with_package()
        {
            var path = new AssetPath("pak1:folder/jquery.js");
            path.Folder.ShouldBeNull();
            path.Name.ShouldEqual("folder/jquery.js");
            path.Package.ShouldEqual("pak1");
        }

        [Test]
        public void deep_path_with_package_and_type()
        {
            var path = new AssetPath("pak1:scripts/folder/jquery.js");
            path.Folder.ShouldEqual(AssetFolder.scripts);
            path.Name.ShouldEqual("folder/jquery.js");
            path.Package.ShouldEqual("pak1");
        }
    }
}