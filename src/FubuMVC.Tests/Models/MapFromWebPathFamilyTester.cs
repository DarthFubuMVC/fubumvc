using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Models
{
    [TestFixture]
    public class MapFromWebPathFamilyTester
    {
        [SetUp]
        public void SetUp()
        {
            family = new MapFromWebPathFamily();
            webAppFolder = "MapFromWebPathFamilyTester";
            UrlContext.Stub(webAppFolder);

            expandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DefaultPath);
            noExpandProp = ReflectionHelper.GetProperty<TestSettings>(t => t.DoNotExpand);
        }

        private MapFromWebPathFamily family;
        private PropertyInfo noExpandProp;
        private PropertyInfo expandProp;
        private string webAppFolder;

        public class TestSettings
        {
            [MapFromWebPath]
            public string DefaultPath { get; set; }

            public string DoNotExpand { get; set; }
        }

        [Test]
        public void resolve_to_full_paths_for_settings_marked_for_local_path_resolution()
        {
            var value = new InMemoryBindingContext().WithPropertyValue("~/file.txt");

            object result = family.Build(null, expandProp)(value);
            result.ShouldEqual(webAppFolder + @"/file.txt");
        }

        [Test]
        public void should_match_properties_with_attribute()
        {
            family.Matches(expandProp).ShouldBeTrue();
        }

        [Test]
        public void should_not_match_properties_without_attribute()
        {
            family.Matches(noExpandProp).ShouldBeFalse();
        }
    }
}