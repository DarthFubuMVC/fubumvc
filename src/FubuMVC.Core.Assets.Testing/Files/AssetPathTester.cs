using FubuMVC.Core.Assets.Files;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets.Files
{
    [TestFixture]
    public class AssetPathTester
    {
        [Test]
        public void is_image()
        {
            new AssetPath("images/pict.bmp").IsBinary().ShouldBeTrue();
            new AssetPath("images/pict.jpg").IsBinary().ShouldBeTrue();
            new AssetPath("images/pict.jpeg").IsBinary().ShouldBeTrue();
            new AssetPath("images/pict.gif").IsBinary().ShouldBeTrue();
            new AssetPath("images/pict.png").IsBinary().ShouldBeTrue();
            new AssetPath("styles/pict.png").IsBinary().ShouldBeTrue();
            new AssetPath("images/pict.unk").IsBinary().ShouldBeTrue(); // Use the folder as is
            new AssetPath("styles/pict.unk").IsBinary().ShouldBeFalse(); // Use the folder as is
            new AssetPath("images/pict.js").IsBinary().ShouldBeFalse();
            new AssetPath("images/pict.css").IsBinary().ShouldBeFalse();
        
        }

        [Test]
        public void to_full_name_with_only_a_name()
        {
            new AssetPath("file.txt").ToFullName().ShouldEqual("file.txt");
        }

        [Test]
        public void to_full_name_when_the_mime_type_can_be_inferred()
        {
            new AssetPath("scripts/script1.js").ToFullName().ShouldEqual("scripts/script1.js");
        }

        [Test]
        public void to_full_name_with_a_package()
        {
            new AssetPath("pak1", "script1.js", AssetFolder.scripts)
                .ToFullName().ShouldEqual("pak1:scripts/script1.js");
        }

        [Test]
        public void to_full_name_with_a_package_and_nested_structure()
        {
            new AssetPath("pak1", "f1/f2/script1.js", AssetFolder.scripts)
                .ToFullName().ShouldEqual("pak1:scripts/f1/f2/script1.js");
        }


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
        public void path_with_package_and_type_specified_by_enumerable()
        {
            var path = new AssetPath(new []{"pak1:scripts", "jquery.js"});
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
        public void deep_path_by_enumerable()
        {
            var path = new AssetPath(new []{"folder","jquery.js"});
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
        public void deep_path_with_type_by_enumerable()
        {
            var path = new AssetPath(new string[]{"scripts","folder","jquery.js"});
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
        public void deep_path_with_package_by_enumerable()
        {
            var path = new AssetPath(new string[]{"pak1:folder", "jquery.js"});
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


        [Test]
        public void deep_path_with_package_and_type_by_enumerable()
        {
            var path = new AssetPath(new string[]{"pak1:scripts","folder","jquery.js"});
            path.Folder.ShouldEqual(AssetFolder.scripts);
            path.Name.ShouldEqual("folder/jquery.js");
            path.Package.ShouldEqual("pak1");
        }
    }
}