using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using FubuMVC.Core.View;
using NUnit.Framework;
using Rhino.Mocks;
using Spark.FileSystem;
using Spark.Web.FubuMVC.Tests.Controllers;
using Spark.Web.FubuMVC.Tests.Helpers;

namespace Spark.Web.FubuMVC.Tests
{
    [TestFixture]
    public class SparkViewFactoryTester
    {
        private SparkViewFactory _factory;
        private ControllerContext _controllerContext;
        private RouteData _routeData;
        private HttpContextBase _httpContext;
        private HttpResponseBase _response;
        private TextWriter _output;
        private object _controller;

        [SetUp]
        public void SetUp()
        {
            CompiledViewHolder.Current = null; //clear the view cache

            var settings = new SparkSettings();
            _factory = new SparkViewFactory(settings) { ViewFolder = new FileSystemViewFolder("FubuMVC.Tests.Views") };

            _httpContext = MockHttpContextBase.Generate("/", new StringWriter());
            _response = _httpContext.Response;
            _output = _response.Output;

            _routeData = new RouteData();
            _routeData.Values.Add("controller", "Stub");
            _routeData.Values.Add("action", "Index");
            _controller = new StubController();
            _controllerContext = new ControllerContext(_httpContext, _routeData, _controller);
        }

        [Test]
        public void should_render_a_plain_view()
        {
            var viewEngineResult = _factory.FindView(_controllerContext, "index", null);
            viewEngineResult.View.RenderView(_output);

            _output.ToString().ShouldEqual("<div>index</div>");
        }

        
    }
}
