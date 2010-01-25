using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Routing;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Rhino.Mocks;
using Spark.FileSystem;
using Spark.Web.FubuMVC.Tests.Controllers;
using Spark.Web.FubuMVC.Tests.Helpers;
using Spark.Web.FubuMVC.Tests.Models;

namespace Spark.Web.FubuMVC.Tests
{
    [TestFixture]
    public class SparkViewFactoryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            CompiledViewHolder.Current = null; //clear the view cache

            var settings = new SparkSettings();
            _factory = new SparkViewFactory(settings) {ViewFolder = new FileSystemViewFolder("FubuMVC.Tests.Views")};

            _httpContext = MockHttpContextBase.Generate("/", new StringWriter());
            _response = _httpContext.Response;
            _output = _response.Output;

            _routeData = new RouteData();
            _routeData.Values.Add("controller", "Stub");
            _routeData.Values.Add("action", "Index");
            _controllerContext = new ControllerContext(_httpContext, _routeData, new StubController());
        }

        #endregion

        private SparkViewFactory _factory;
        private ControllerContext _controllerContext;
        private RouteData _routeData;
        private HttpContextBase _httpContext;
        private HttpResponseBase _response;
        private TextWriter _output;

        private void FindPartialViewAndRender(string partialViewName)
        {
            ViewEngineResult viewEngineResult = _factory.FindPartialView(_controllerContext, partialViewName);
            viewEngineResult.View.RenderView(_output);
        }

        private void FindViewAndRender(string viewName)
        {
            FindViewAndRender(viewName, null);
        }

        private void FindViewAndRender<T>(string viewName, T viewModel) where T : class
        {
            ViewEngineResult viewEngineResult = _factory.FindView(_controllerContext, viewName, null);
            var sparkView = viewEngineResult.View as SparkView<T>;
            if (sparkView != null)
            {
                var request = MockRepository.GenerateStub<IFubuRequest>();
                request.Expect(x => x.Get<T>()).Return(viewModel);
                sparkView.SetModel(request);
            }
            viewEngineResult.View.RenderView(_output);
        }

        private void FindViewAndRender(string viewName, string masterName)
        {
            ViewEngineResult viewEngineResult = _factory.FindView(_controllerContext, viewName, masterName);
            viewEngineResult.View.RenderView(_output);
        }

        public class ScopedCulture : IDisposable
        {
            private readonly CultureInfo savedCulture;

            public ScopedCulture(CultureInfo culture)
            {
                savedCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = culture;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Thread.CurrentThread.CurrentCulture = savedCulture;
            }

            #endregion
        }

        private static void ContainsInOrder(string content, params string[] values)
        {
            int index = 0;
            foreach (string value in values)
            {
                int nextIndex = content.IndexOf(value, index);
                Assert.GreaterOrEqual(nextIndex, 0, string.Format("Looking for {0}", value));
                index = nextIndex + value.Length;
            }
        }

        [Test]
        public void application_dot_spark_should_be_used_as_the_master_layout_if_present()
        {
            _factory.ViewFolder = new InMemoryViewFolder
                                      {
                                          {"Foo\\baz.spark", ""},
                                          {"Shared\\Application.spark", ""}
                                      };

            _routeData.Values["controller"] = "Foo";
            _routeData.Values["action"] = "Notbaz";

            SparkViewDescriptor descriptor = _factory.CreateDescriptor(_controllerContext, "baz", null, true, null);

            descriptor.Templates.ShouldHaveCount(2);
            descriptor.Templates[0].ShouldEqual("Foo\\baz.spark");
            descriptor.Templates[1].ShouldEqual("Shared\\Application.spark");
        }

        [Test]
        public void controllername_dot_spark_should_be_used_as_its_master_layout_if_present()
        {
            _factory.ViewFolder = new InMemoryViewFolder
                                      {
                                          {"Foo\\baz.spark", ""},
                                          {"Shared\\Foo.spark", ""}
                                      };

            _routeData.Values["controller"] = "Foo";
            _routeData.Values["action"] = "Notbaz";

            SparkViewDescriptor descriptor = _factory.CreateDescriptor(_controllerContext, "baz", null, true, null);

            descriptor.Templates.ShouldHaveCount(2);
            descriptor.Templates[0].ShouldEqual("Foo\\baz.spark");
            descriptor.Templates[1].ShouldEqual("Shared\\Foo.spark");
        }

        [Test]
        public void should_be_able_to_change_view_source_folder_on_the_fly()
        {
            var replacement = MockRepository.GenerateStub<IViewFolder>();

            IViewFolder existing = _factory.ViewFolder;
            existing.ShouldNotBeTheSameAs(replacement);
            existing.ShouldBeTheSameAs(_factory.ViewFolder);

            _factory.ViewFolder = replacement;
            replacement.ShouldBeTheSameAs(_factory.ViewFolder);
            existing.ShouldNotBeTheSameAs(_factory.ViewFolder);
        }

        [Test]
        public void should_be_able_to_get_the_target_namespace_from_the_controller()
        {
            _factory.ViewFolder = new InMemoryViewFolder
                                      {
                                          {"Stub\\Foo.spark", ""},
                                          {"Layouts\\Home.spark", ""}
                                      };

            SparkViewDescriptor descriptor = _factory.CreateDescriptor(_controllerContext, "Foo", null, true, null);

            Assert.AreEqual("Spark.Web.FubuMVC.Tests.Controllers", descriptor.TargetNamespace);
        }

        [Test]
        public void should_be_able_to_html_encode_using_H_function_from_views()
        {
            FindViewAndRender("ViewThatUsesHtmlEncoding");

            _output.ToString().Replace(" ", "").Replace("\r", "").Replace("\n", "")
                .ShouldEqual("<div>&lt;div&gt;&amp;lt;&amp;gt;&lt;/div&gt;</div>");
        }

        [Test]
        public void should_be_able_to_locate_partial_view_in_an_area()
        {
            _controllerContext.RouteData.Values.Add("area", "SomeFooArea");
            FindPartialViewAndRender("index");

            _output.ToString().ShouldEqual("<div>default view some foo area</div>");
        }

        [Test]
        public void should_be_able_to_locate_view_in_an_area()
        {
            _controllerContext.RouteData.Values.Add("area", "SomeFooArea");
            FindViewAndRender("index");
            _output.ToString().ShouldEqual("<div>default view some foo area</div>");
        }

        [Test]
        public void should_be_able_to_locate_view_in_an_area_with_a_layout()
        {
            _controllerContext.RouteData.Values.Add("area", "SomeFooArea");
            FindViewAndRender("index", "layout");

            _output.ToString().ShouldContainInOrder("<body>", "<div>default view some foo area</div>", "</body>");
        }

        [Test]
        public void should_be_able_to_locate_view_in_an_area_with_a_layout_in_the_same_area()
        {
            _controllerContext.RouteData.Values.Add("area", "SomeFooArea");
            FindViewAndRender("index", "fooAreaLayout");

            ContainsInOrder(_output.ToString(),
                            "<body class=\"fooArea\">",
                            "<div>default view some foo area</div>",
                            "</body>");
        }

        [Test]
        public void should_be_able_to_provide_global_setting_for_views()
        {
            FindViewAndRender("ViewThatChangesGlobalSettings", null);

            _output.ToString().ShouldContainInOrder(
                "<div>default: Global set test</div>",
                "<div>7==7</div>");
        }

        [Test]
        public void should_be_able_to_render_a_child_view_with_a_master_layout()
        {
            FindViewAndRender("ChildViewThatExpectsALayout", "Layout");

            _output.ToString().ShouldContainInOrder(
                "<title>Child View That Expects A Layout</title>",
                "<div>no header by default</div>",
                "<h1>Child View That Expects A Layout</h1>",
                "<div>no footer by default</div>");
        }

        [Test]
        public void should_be_able_to_render_a_plain_view()
        {
            ViewEngineResult viewEngineResult = _factory.FindView(_controllerContext, "index", null);
            viewEngineResult.View.RenderView(_output);

            _output.ToString().ShouldEqual("<div>index</div>");
        }

        [Test]
        public void should_be_able_to_render_a_view_even_with_null_view_model()
        {
            FindViewAndRender("ViewThatUsesANullViewModel");

            _output.ToString().ShouldContain("<div>nothing</div>");
        }

        [Test]
        public void should_be_able_to_render_a_view_with_a_strongly_typed_model()
        {
            FindViewAndRender("ViewThatUsesViewModel", new FakeViewModel {Text = "Spark"});
            _output.ToString().ShouldContain("<div>Spark</div>");
        }

        [Test, Ignore("Only add this is we think we'll need to render anonymous types")]
        public void should_be_able_to_render_a_view_with_an_anonymous_view_model()
        {
            FindViewAndRender("ViewThatUsesAnonymousViewModel", new {Foo = 42, Bar = new FakeViewModel {Text = "is the answer"}});

            _output.ToString().ShouldContain("<div>42 is the answer</div>");
        }

        [Test, Ignore("Only add this is we think we'll need to render anonymous types")]
        public void should_be_able_to_render_a_view_with_culture_aware_formatting()
        {
            using (new ScopedCulture(CultureInfo.CreateSpecificCulture("en-us")))
            {
                FindViewAndRender("ViewThatUsesFormatting", new {Number = 9876543.21, Date = new DateTime(2010, 12, 11)});

                _output.ToString().ShouldContainInOrder(
                    "<div>9,876,543.21</div>",
                    "<div>2010/12/11</div>");
            }
        }

        [Test]
        public void should_be_able_to_render_partials_that_share_state()
        {
            FindViewAndRender("ViewThatRendersPartialsThatShareState");

            _output.ToString().ShouldContainInOrder(
                "<div>start</div>",
                "<div>lion</div>",
                "<div>elephant</div>",
                "<div>The Target</div>",
                "<div>Willow</div>",
                "<div>middle</div>",
                "<ul>",
                "<li>one</li>",
                "<li>three</li>",
                "<li>two</li>",
                "</ul>",
                "alphabetagammadelta",
                "<div>end</div>");
            _output.ToString().ShouldNotContain("foo2");
            _output.ToString().ShouldNotContain("bar4");
            _output.ToString().ShouldNotContain("quux7");
        }

        [Test]
        public void should_be_able_to_use_a_partial_file_explicitly()
        {
            FindViewAndRender("ViewThatUsesPartial");

            _output.ToString().ShouldContainInOrder(
                "<ul>",
                "<li>Partial where x=\"lion\"</li>",
                "<li>Partial where x=\"hippo\"</li>",
                "<li>Partial where x=\"elephant\"</li>",
                "<li>Partial where x=\"giraffe\"</li>",
                "<li>Partial where x=\"whale\"</li>",
                "</ul>");
        }

        [Test]
        public void should_be_able_to_use_a_partial_file_implicitly()
        {
            FindViewAndRender("ViewThatUsesPartialImplicitly");

            Console.WriteLine(_output.ToString());
            _output.ToString().ShouldContainInOrder(
                "<li class=\"odd\">lion</li>",
                "<li class=\"even\">hippo</li>");
        }

        [Test]
        public void should_be_able_to_use_foreach_construct_in_the_view()
        {
            FindViewAndRender("ViewThatUsesForeach");

            _output.ToString().ShouldContainInOrder(
                "<li class=\"odd\">1: foo</li>",
                "<li class=\"even\">2: bar</li>",
                "<li class=\"odd\">3: baz</li>");
        }

        [Test]
        public void should_be_able_to_use_namespaces_directly()
        {
            FindViewAndRender("ViewThatUsesNamespaces");

            _output.ToString().ShouldContainInOrder(
                "<div>Foo</div>",
                "<div>Bar</div>",
                "<div>Hello</div>");
        }

        [Test, Ignore("Need to figure out why it can't see the reference to HtmlTags.dll")]
        public void should_be_able_to_use_the_html_tags_assembly()
        {
            ((SparkSettings) _factory.Settings).AddNamespace("HtmlTags");
            FindViewAndRender("HtmlTags", null);

            string content = _output.ToString();
            Assert.That(content.Contains("<div><a href=\"/Stub/List\">Back to List</a></div>"));
            Assert.That(content.Contains("<div>foo&gt;bar</div>"));
        }

        [Test]
        public void should_capture_named_content_areas_and_render_in_the_correct_order()
        {
            FindViewAndRender("ChildViewThatUsesAllNamedContentAreas", "Layout");

            _output.ToString().ShouldContainInOrder(
                "<div>Funny, we can put the header anywhere we like with a name</div>",
                "<div>OK - this is the main content by default because it is not contained</div>",
                "<div>Here is some footer stuff defined at the top</div>",
                "<div>Much better place for footer stuff - or is it?</div>");
        }

        [Test]
        public void the_master_layout_should_be_empty_by_default()
        {
            _factory.ViewFolder = new InMemoryViewFolder
                                      {
                                          {"Foo\\baz.spark", ""}
                                      };

            _routeData.Values["controller"] = "Foo";
            _routeData.Values["action"] = "Notbaz";

            SparkViewDescriptor descriptor = _factory.CreateDescriptor(_controllerContext, "baz", null, true, null);

            descriptor.Templates.ShouldHaveCount(1);
            descriptor.Templates[0].ShouldEqual("Foo\\baz.spark");
        }
    }
}