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
            var theHttpRequest = MockRepository.GenerateMock<ICurrentChain>();
            theHttpRequest.Stub(x => x.ResourceHash()).Return("something/else");

            var locator = new StructureMapServiceLocator(new Container(x =>
            {
                x.For<ICurrentChain>().Use(theHttpRequest);
            }));

            FubuApplication.SetupNamingStrategyForHttpHeaders();

            var data = new InMemoryRequestData();
            data["If-None-Match"] = "12345";


            Assert.Fail("Switch to BindingScenario");

            //var context = new BindingContext(data, locator, new NulloBindingLogger());

            //var binder = StandardModelBinder.Basic();

            //theEtagRequest =  binder.Bind(typeof(ETaggedRequest), context).As<ETaggedRequest>();
        }


        [Test]
        public void bind_the_resource_path()
        {
            theEtagRequest.ResourceHash.ShouldEqual("something/else");
        }

        [Test]
        public void binds_the_header_value()
        {
            theEtagRequest.IfNoneMatch.ShouldEqual("12345");
        }
    }
}