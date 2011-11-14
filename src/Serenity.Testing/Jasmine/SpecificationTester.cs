using FubuMVC.Core.Assets.Files;
using NUnit.Framework;
using Serenity.Jasmine;
using FubuTestingSupport;
using System.Linq;

namespace Serenity.Testing.Jasmine
{
    [TestFixture]
    public class SpecificationTester
    {

        private bool isDependency(string specName, string assetName)
        {
            var specification = new Specification(new AssetFile(specName));

            return specification.DependsOn(new AssetFile(assetName));
        }

        [TearDown]
        public void Teardown()
        {
            Specification.RebuildIgnoredExtensions();
        }

        [Test]
        public void is_specification()
        {
            Specification.IsSpecification(new AssetFile("specs/something.js")).ShouldBeTrue();
            Specification.IsSpecification(new AssetFile("specs/folder/something.js")).ShouldBeTrue();
            Specification.IsSpecification(new AssetFile("folder/something.spec.js")).ShouldBeFalse();
        }

        [Test]
        public void determine_library_name()
        {
            Specification.DetermineLibraryName(new AssetFile("lib1.js")).ShouldEqual("lib1");
            Specification.DetermineLibraryName(new AssetFile("jquery.forms.js")).ShouldEqual("jquery.forms");
            Specification.DetermineLibraryName(new AssetFile("shared/jquery.forms.js")).ShouldEqual("jquery.forms");

            Specification.IgnoreExtension(".coffee");

            Specification.DetermineLibraryName(new AssetFile("shared/my.system.lib.coffee.js"))
                .ShouldEqual("my.system.lib");
        }

        [Test]
        public void javascript_extension_is_ignored_by_default()
        {
            Specification.RebuildIgnoredExtensions();
            Specification.IgnoredExtensions.ShouldContain(".js");
        }

        [Test]
        public void spec_extension_is_ignored_by_default()
        {
            Specification.RebuildIgnoredExtensions();
            Specification.IgnoredExtensions.ShouldContain(".spec");
        }

        [Test]
        public void content_folder()
        {
            new Specification("specs/lib1.spec.js").ContentFolder.ShouldBeNull();
            new Specification("specs/f1/lib1.spec.js").ContentFolder.ShouldEqual("f1");
            new Specification("specs/f1/f2/lib1.spec.js").ContentFolder.ShouldEqual("f1/f2");
            new Specification("f1/specs/lib1.spec.js").ContentFolder.ShouldEqual("f1");
        }

        [Test]
        public void subject()
        {


            new Specification("specs/lib1.spec.js").Subject.ShouldEqual("lib1");
            new Specification("specs/lib1.spec.coffee").Subject.ShouldEqual("lib1");
            new Specification("specs/f1/lib1.spec.js").Subject.ShouldEqual("lib1");
            new Specification("specs/f1/f2/lib1.spec.js").Subject.ShouldEqual("lib1");
            new Specification("specs/f1/f2/lib1.other.spec.js").Subject.ShouldEqual("lib1.other");
        }

        [Test]
        public void depends_on_simple_cases()
        {
            isDependency("specs/lib1.spec.js", "lib1.js").ShouldBeTrue();
            isDependency("specs/folder/folder2/lib1.spec.js", "folder/folder2/lib1.js").ShouldBeTrue();

            isDependency("specs/lib2.spec.js", "lib1.js").ShouldBeFalse();
            isDependency("specs/lib1.spec.js", "lib2.js").ShouldBeFalse();
            isDependency("specs/folder2/lib1.spec.js", "folder/lib1.js").ShouldBeFalse();
            isDependency("specs/folder2/lib1.spec.js", "folder/folder2/lib1.js").ShouldBeFalse();
        }

        [Test]
        public void depends_on_simple_cases_deep_folder()
        {

            isDependency("specs/folder/lib1.spec.js", "folder/lib1.js").ShouldBeTrue();

        }

        [Test]
        public void depends_on_simple_cases_is_case_insensitive()
        {
            isDependency("specs/Lib1.spec.js", "lib1.js").ShouldBeTrue();
            isDependency("specs/lib1.spec.js", "Lib1.js").ShouldBeTrue();
            isDependency("specs/Folder/lib1.spec.js", "folder/lib1.js").ShouldBeTrue();
            isDependency("specs/folder/folder2/lib1.spec.js", "Folder/folder2/lib1.js").ShouldBeTrue();

            isDependency("specs/lib2.spec.js", "Lib1.js").ShouldBeFalse();
            isDependency("specs/liB1.spec.js", "lib2.js").ShouldBeFalse();
            isDependency("specs/Folder2/lib1.spec.js", "folder/lib1.js").ShouldBeFalse();
            isDependency("specs/folder2/lib1.spec.js", "Folder/folder2/lib1.js").ShouldBeFalse();
        }

        [Test]
        public void depends_on_complex_names()
        {
            isDependency("specs/jquery.forms.spec.js", "jquery.forms.js").ShouldBeTrue();
            isDependency("specs/jquery.spec.js", "jquery.js").ShouldBeTrue();
            
        }

        [Test]
        public void can_use_ignored_extensions_to_be_more_exact_in_matching()
        {
            isDependency("specs/jquery.spec.js", "jquery.forms.js").ShouldBeFalse();
        }

        [Test]
        public void select_html_files()
        {
            var spec = new Specification("somelib.spec.js");

            var files = new System.Collections.Generic.List<AssetFile>(){
                new AssetFile("not.fixture.html"),
                new AssetFile("somelib.css"),
                new AssetFile("somelib.fixture.html")
            };

            spec.SelectHtmlFiles(files);
            spec.HtmlFiles.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("somelib.fixture.html");
        
        }
    }
}