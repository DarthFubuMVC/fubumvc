using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.StructureMap;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;
using FubuTestingSupport;

namespace FubuMVC.Tests.Resources.Etags
{
    [TestFixture]
    public class ModelBindingETaggedRequestTester
    {
        private ETaggedRequest theEtagRequest;

        [SetUp]
        public void SetUp()
        {
            var theHttpRequest = MockRepository.GenerateMock<ICurrentHttpRequest>();
            theHttpRequest.Stub(x => x.RelativeUrl()).Return("something/else?key=value");

            var locator = new StructureMapServiceLocator(new Container(x =>
            {
                x.For<ICurrentHttpRequest>().Use(theHttpRequest);
            }));

            FubuApplication.SetupNamingStrategyForHttpHeaders();

            var data = new InMemoryRequestData();
            data["If-None-Match"] = "12345";

            var context = new BindingContext(data, locator, new NulloBindingLogger());

            var binder = StandardModelBinder.Basic();

            theEtagRequest =  binder.Bind(typeof(ETaggedRequest), context).As<ETaggedRequest>();
        }


        [Test]
        public void bind_the_resource_path()
        {
            theEtagRequest.ResourcePath.ShouldEqual("something/else");
        }

        [Test]
        public void binds_the_header_value()
        {
            theEtagRequest.IfNoneMatch.ShouldEqual("12345");
        }
    }
}