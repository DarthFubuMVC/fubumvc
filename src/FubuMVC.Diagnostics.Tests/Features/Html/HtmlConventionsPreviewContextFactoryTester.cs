using System;
using System.Linq;
using FubuCore;
using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Features.Html
{
    [TestFixture]
    public class HtmlConventionsPreviewContextFactoryTester : InteractionContext<HtmlConventionsPreviewContextFactory>
    {
        private Type _modelType;

        protected override void beforeEach()
        {
            _modelType = typeof (SampleContextModel);

            MockFor<IPreviewModelTypeResolver>()
                .Expect(r => r.TypeFor(Arg<string>.Is.Anything))
                .Return(_modelType);

            MockFor<IPreviewModelActivator>()
                .Expect(a => a.Activate(_modelType))
                .Return(new SampleContextModel());
        }

        [Test]
        public void should_parse_model_type()
        {
            ClassUnderTest
                .BuildFromPath(_modelType.FullName)
                .ModelType
                .ShouldEqual(_modelType);
        }

        [Test]
        public void should_parse_property_paths()
        {
            var path = "{0}-Child".ToFormat(_modelType.FullName);
            ClassUnderTest
                .BuildFromPath(path)
                .PropertyChain
                .First()
                .Name
                .ShouldEqual("Child");
        }
    }
}