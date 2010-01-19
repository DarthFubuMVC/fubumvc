using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Spark.FileSystem;
using Spark.Parser;
using Spark.Web.FubuMVC.Tests.Controllers;
using Spark.Web.FubuMVC.ViewLocation;

namespace Spark.Web.FubuMVC.Tests
{
    [TestFixture]
    public class SparkDescriptorBuildingTester
    {
        #region Setup/Teardown

        [SetUp]
        public void Init()
        {
            CompiledViewHolder.Current = null;

            _factory = new SparkViewFactory();
            _viewFolder = new InMemoryViewFolder();
            _factory.ViewFolder = _viewFolder;
            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            _routeData = new RouteData();
            var controller = new StubController();
            _controllerContext = new ControllerContext(httpContext, _routeData, controller);
        }

        #endregion

        private SparkViewFactory _factory;
        private InMemoryViewFolder _viewFolder;
        private RouteData _routeData;
        private ControllerContext _controllerContext;

        private static void AssertDescriptorTemplates(SparkViewDescriptor descriptor, IEnumerable<string> searchedLocations, params string[] templates)
        {
            templates.ShouldHaveCount(descriptor.Templates.Count);
            for (int index = 0; index != templates.Length; ++index)
                templates[index].ShouldEqual(descriptor.Templates[index]);
            searchedLocations.ShouldHaveCount(0);
        }

        private static IDictionary<string, object> Dict(IEnumerable<string> values)
        {
            return values == null
                       ? null
                       : values.Select((v, k) => new {k, v}).ToDictionary(kv => kv.k.ToString(), kv => (object) kv.v);
        }

        [Test]
        public void build_descriptor_extra_params_set_to_null_should_acts_as_empty()
        {
            var param1 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, null);
            var param2 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, new Dictionary<string, object>());

            Assert.That(param1, Is.EqualTo(param2));
        }

        [Test]
        public void build_descriptor_extra_params_should_have_identical_equality()
        {
            var param1 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, Dict(new[] {"hippo", "lion"}));
            var param2 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, Dict(new[] {"hippo"}));
            var param3 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, Dict(new[] {"lion"}));
            var param4 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, Dict(new[] {"lion", "hippo"}));
            var param5 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, Dict(null));
            var param6 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, Dict(new string[0]));
            var param7 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, Dict(new[] {"hippo", "lion"}));

            Assert.That(param1, Is.Not.EqualTo(param2));
            Assert.That(param1, Is.Not.EqualTo(param3));
            Assert.That(param1, Is.Not.EqualTo(param4));
            Assert.That(param1, Is.Not.EqualTo(param5));
            Assert.That(param1, Is.Not.EqualTo(param6));
            Assert.That(param1, Is.EqualTo(param7));
        }

        [Test]
        public void build_descriptor_params_should_act_as_a_unique_key()
        {
            var param1 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, null);
            var param2 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", false, null);
            var param3 = new BuildDescriptorParams("foo", "bar", "fizz", "buzz", true, null);
            var param4 = new BuildDescriptorParams("foo", "baz", "fizz", "buzz", false, null);

            param1.ShouldEqual(param2);
            param1.ShouldNotEqual(param3);
            param1.ShouldNotEqual(param4);
        }

        [Test, ExpectedException(typeof (InvalidCastException))]
        public void custom_descriptor_builders_should_not_be_able_to_use_descriptor_filters()
        {
            _factory.DescriptorBuilder = MockRepository.GenerateStub<IDescriptorBuilder>();
            _factory.AddFilter(MockRepository.GenerateStub<IDescriptorFilter>());
        }

        [Test]
        public void descriptors_with_custom_parameter_should_be_added_to_the_view_search_path()
        {
            _factory.DescriptorBuilder = new ExtendingDescriptorBuilderWithInheritance(_factory.Engine);
            _routeData.Values.Add("controller", "Bar");
            _routeData.Values.Add("language", "en-gb");
            _viewFolder.Add(@"Bar\Index.en-gb.spark", "");
            _viewFolder.Add(@"Bar\Index.en.spark", "");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Application.en.spark", "");
            _viewFolder.Add(@"Layouts\Application.it.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.en-gb.spark", @"Layouts\Application.en.spark");
        }

        [Test]
        public void descriptors_with_named_master_should_override_the_view_master()
        {
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "<use master='Lion'/>");
            _viewFolder.Add(@"Layouts\Elephant.spark", "<use master='Whale'/>");
            _viewFolder.Add(@"Layouts\Lion.spark", "<use master='Elephant'/>");
            _viewFolder.Add(@"Layouts\Whale.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"Layouts\Bar.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", "Elephant", true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark", @"Layouts\Elephant.spark", @"Layouts\Whale.spark");
        }

        [Test]
        public void descriptors_with_normal_view_and_controller_layout_overrides()
        {
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"Layouts\Bar.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark", @"Layouts\Bar.spark");
        }

        [Test]
        public void descriptors_with_normal_view_and_default_layout_present()
        {
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark", @"Layouts\Application.spark");
        }

        [Test]
        public void descriptors_with_normal_view_and_named_master()
        {
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"Layouts\Home.spark", "");
            _viewFolder.Add(@"Layouts\Site.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", "Site", true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark", @"Layouts\Site.spark");
        }

        [Test]
        public void descriptors_with_normal_view_and_no_default_layout()
        {
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark");
        }

        [Test]
        public void descriptors_with_partial_view_from_area_should_ignore_layout()
        {
            _routeData.Values.Add("area", "SomeFooArea");
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"SomeFooArea\Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Bar.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"SomeFooArea\Layouts\Application.spark", "");
            _viewFolder.Add(@"SomeFooArea\Layouts\Bar.spark", "");
            _viewFolder.Add(@"SomeFooArea\Layouts\Site.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, false, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"SomeFooArea\Bar\Index.spark");
        }

        [Test]
        public void descriptors_with_partial_view_should_ignore_default_layouts()
        {
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"Layouts\Home.spark", "");
            _viewFolder.Add(@"Shared\Application.spark", "");
            _viewFolder.Add(@"Shared\Home.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, false, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark");
        }

        [Test]
        public void descriptors_with_partial_view_should_ignore_use_master_and_default()
        {
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "<use master='Lion'/>");
            _viewFolder.Add(@"Layouts\Elephant.spark", "<use master='Whale'/>");
            _viewFolder.Add(@"Layouts\Lion.spark", "<use master='Elephant'/>");
            _viewFolder.Add(@"Layouts\Whale.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"Layouts\Bar.spark", "");

            var searchedLocations = new List<string>();
            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, false, searchedLocations);
            AssertDescriptorTemplates(result, searchedLocations, @"Bar\Index.spark");
        }

        [Test]
        public void descriptors_with_route_area_present_should_default_to_normal_location()
        {
            _routeData.Values.Add("area", "SomeFooArea");
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"Bar\Index.spark", @"Layouts\Application.spark");
        }

        [Test]
        public void descriptors_with_simplified_use_master_grammar_should_detect_element_correctly()
        {
            var builder = new FubuDescriptorBuilder();

            ParseResult<string> a = builder.ParseUseMaster(new Position(new SourceContext("<use master='a'/>")));
            ParseResult<string> b = builder.ParseUseMaster(new Position(new SourceContext("<use\r\nmaster \r\n =\r\n'b' />")));
            ParseResult<string> c = builder.ParseUseMaster(new Position(new SourceContext("<use master=\"c\"/>")));
            ParseResult<string> def = builder.ParseUseMaster(new Position(new SourceContext("  x <use etc=''/> <use master=\"def\"/> y ")));
            ParseResult<string> none = builder.ParseUseMaster(new Position(new SourceContext("  x <use etc=''/> <using master=\"def\"/> y ")));
            ParseResult<string> g = builder.ParseUseMaster(new Position(new SourceContext("-<use master=\"g\"/>-<use master=\"h\"/>-")));

            a.Value.ShouldEqual("a");
            b.Value.ShouldEqual("b");
            c.Value.ShouldEqual("c");
            def.Value.ShouldEqual("def");
            none.ShouldBeNull();
            g.Value.ShouldEqual("g");
        }

        [Test]
        public void descriptors_with_some_foo_area_folder_could_contain_controller_folder()
        {
            _routeData.Values.Add("area", "SomeFooArea");
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"SomeFooArea\Bar\Index.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"SomeFooArea\Bar\Index.spark", @"Layouts\Application.spark");
        }

        [Test]
        public void descriptors_with_some_foo_area_folder_could_contain_layouts_folder()
        {
            _routeData.Values.Add("area", "SomeFooArea");
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"SomeFooArea\Bar\Index.spark", "");
            _viewFolder.Add(@"SomeFooArea\Layouts\Application.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"SomeFooArea\Bar\Index.spark", @"SomeFooArea\Layouts\Application.spark");
        }

        [Test]
        public void descriptors_with_some_foo_area_should_contain_named_layout()
        {
            _routeData.Values.Add("area", "SomeFooArea");
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"SomeFooArea\Bar\Index.spark", "");
            _viewFolder.Add(@"SomeFooArea\Layouts\Application.spark", "");
            _viewFolder.Add(@"SomeFooArea\Layouts\Site.spark", "");

            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", "Site", true, new List<string>());
            AssertDescriptorTemplates(result, new List<string>(), @"SomeFooArea\Bar\Index.spark", @"SomeFooArea\Layouts\Site.spark");
        }

        [Test]
        public void descriptors_with_use_master_should_create_a_template_chain()
        {
            _routeData.Values.Add("controller", "Bar");
            _viewFolder.Add(@"Bar\Index.spark", "<use master='Lion'/>");
            _viewFolder.Add(@"Layouts\Elephant.spark", "<use master='Whale'/>");
            _viewFolder.Add(@"Layouts\Lion.spark", "<use master='Elephant'/>");
            _viewFolder.Add(@"Layouts\Whale.spark", "");
            _viewFolder.Add(@"Layouts\Application.spark", "");
            _viewFolder.Add(@"Layouts\Bar.spark", "");

            var searchedLocations = new List<string>();
            SparkViewDescriptor result = _factory.CreateDescriptor(_controllerContext, "Index", null, true, searchedLocations);
            AssertDescriptorTemplates(result, searchedLocations, @"Bar\Index.spark", @"Layouts\Lion.spark", @"Layouts\Elephant.spark", @"Layouts\Whale.spark");
        }
    }

    internal class ExtendingDescriptorBuilderWithInheritance : FubuDescriptorBuilder
    {
        public ExtendingDescriptorBuilderWithInheritance(ISparkViewEngine engine)
            : base(engine)
        {
        }

        public override IDictionary<string, object> GetExtraParameters(ControllerContext controllerContext)
        {
            return new Dictionary<string, object>
                       {
                           {"language", Convert.ToString(controllerContext.RouteData.Values["language"])}
                       };
        }

        protected override IEnumerable<string> PotentialViewLocations(string controllerName, string viewName, IDictionary<string, object> extra)
        {
            return Merge(base.PotentialViewLocations(controllerName, viewName, extra), extra["language"].ToString());
        }

        protected override IEnumerable<string> PotentialMasterLocations(string masterName, IDictionary<string, object> extra)
        {
            return Merge(base.PotentialMasterLocations(masterName, extra), extra["language"].ToString());
        }

        protected override IEnumerable<string> PotentialDefaultMasterLocations(string controllerName, IDictionary<string, object> extra)
        {
            return Merge(base.PotentialDefaultMasterLocations(controllerName, extra), extra["language"].ToString());
        }

        private static IEnumerable<string> Merge(IEnumerable<string> locations, string region)
        {
            int slashPos = (region ?? "").IndexOf('-');
            if (region != null)
            {
                string language = slashPos == -1 ? null : region.Substring(0, slashPos);

                foreach (string location in locations)
                {
                    if (!string.IsNullOrEmpty(region))
                    {
                        yield return Path.ChangeExtension(location, region + ".spark");
                        if (slashPos != -1)
                            yield return Path.ChangeExtension(location, language + ".spark");
                    }
                    yield return location;
                }
            }
        }
    }
}