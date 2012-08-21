using System.Collections.Generic;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuMVC.Core;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
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
        private string theExpectedHash;

        [SetUp]
        public void SetUp()
        {
            var theHttpRequest = new CurrentChain(new BehaviorChain(), new Dictionary<string, object>());
            theExpectedHash = ResourceHash.For(theHttpRequest);

            FubuApplication.SetupNamingStrategyForHttpHeaders();

             theEtagRequest = BindingScenario<ETaggedRequest>.Build(x =>
            {
                x.Service<ICurrentChain>(theHttpRequest);
                x.Data("If-None-Match", "12345");
            });
        }


        [Test]
        public void bind_the_resource_path()
        {
            theEtagRequest.ResourceHash.ShouldEqual(theExpectedHash);
        }

        [Test]
        public void binds_the_header_value()
        {
            theEtagRequest.IfNoneMatch.ShouldEqual("12345");
        }
    }
}