using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class TemplateRegistryTester : InteractionContext<SparkTemplateRegistry>
    {
        private IList<ISparkTemplate> _templates;
        private ISparkTemplate[] _bindings;
        protected override void beforeEach()
        {
            _templates = new List<ISparkTemplate>
            {
                new SparkTemplate("App/Shared/bindings.xml", "App", ContentFolder.Application),
                new SparkTemplate("App/bindings.xml", "App", ContentFolder.Application),
                new SparkTemplate("App/Views/binding.xml", "App", ContentFolder.Application),
                new SparkTemplate("App/Actions/binding.xml", "App", ContentFolder.Application),
                new SparkTemplate("App/Actions/Home/home.spark", "App", ContentFolder.Application),
                new SparkTemplate("App/Packages1/Views/Home/home.spark", "App/Package1", "Package1"),
                new SparkTemplate("App/Packages1/Views/Products/list.spark", "App/Package1", "Package1"),
                new SparkTemplate("App/Views/Home/home.spark", "App", ContentFolder.Application)
            };

            _bindings = new[] { _templates[0], _templates[1], _templates[2] };

            var view = _templates.Last();
            view.As<SparkTemplate>().ViewPath = view.FilePath;
            var descriptor = new SparkDescriptor(view, new SparkViewEngine());
            _bindings.Each(descriptor.AddBinding);
            view.Descriptor = descriptor;

            Services.Inject(new SparkTemplateRegistry(_templates));
        }

        [Test]
        public void bindings_from_view_returns_the_descriptor_bindings_of_the_view_that_match_the_view_path()
        {
            ClassUnderTest.BindingsForView("App/Views/Home/home.spark").ShouldEqual(_bindings);
            ClassUnderTest.BindingsForView("App/Actions/Home/home.spark").ShouldHaveCount(0);
            ClassUnderTest.BindingsForView("App/Packages2/Views/Home/home.spark").ShouldHaveCount(0);
        }

        [Test]
        public void first_by_name()
        {
            ClassUnderTest.FirstByName("home").ShouldNotBeNull().FilePath.ShouldEqual("App/Actions/Home/home.spark");
            ClassUnderTest.FirstByName("products").ShouldBeNull();
        }

        [Test]
        public void by_origin()
        {
            ClassUnderTest.ByOrigin("Package1").ShouldHaveCount(2)
                .All(x => x.Origin == "Package1").ShouldBeTrue();
            ClassUnderTest.ByOrigin(ContentFolder.Application).ShouldHaveCount(6)
                .All(x => x.Origin == ContentFolder.Application).ShouldBeTrue();
        }

        [Test]
        public void all_templates()
        {
            ClassUnderTest.ShouldHaveCount(8).ShouldEqual(_templates);
        }

        [Test]
        public void from_host()
        {
            var fromHost = ClassUnderTest.FromHost();
            fromHost.ShouldHaveCount(6);
            fromHost.ShouldEqual(ClassUnderTest.ByOrigin(ContentFolder.Application));
        }

    }
}