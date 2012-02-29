using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Configuration;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Diagnostics.Features.Html;
using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuMVC.Diagnostics.Features.Html.Preview.Decorators;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using HtmlTags;

using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Features.Html.Decorators
{
    [TestFixture]
    public class PropertyExamplesBuilderTester : InteractionContext<PropertyExamplesBuilder>
    {
        private HtmlConventionsPreviewContext _context;
        private HtmlConventionsPreviewViewModel _model;

        protected override void beforeEach()
        {
            _context = ObjectMother.BasicPreviewContext();
            _model = new HtmlConventionsPreviewViewModel();

            Container
                .Configure(x =>
                               {
                                   x.For<IServiceLocator>().Use(() => new StructureMapServiceLocator(Container));
                               });

            MockFor<ITagGeneratorFactory>()
                .Expect(f => f.GeneratorFor(_context.ModelType))
                .Return(MockFor<ITagGenerator<SampleContextModel>>());

            MockFor<ITagGenerator<SampleContextModel>>()
                .Expect(t => t.GetRequest(Arg<Accessor>.Is.Anything))
                .Return(new ElementRequest(_context.Instance,
                                           ReflectionHelper.GetAccessor<SampleContextModel>(m => m.Public),
                                           MockFor<IServiceLocator>()))
                .Repeat
                .Any();

            MockFor<ITagGenerator<SampleContextModel>>()
                .Expect(t => t.LabelFor(Arg<ElementRequest>.Is.Anything))
                .Return(new HtmlTag("label"));

            MockFor<ITagGenerator<SampleContextModel>>()
                .Expect(t => t.InputFor(Arg<ElementRequest>.Is.Anything))
                .Return(new HtmlTag("input"));

            MockFor<ITagGenerator<SampleContextModel>>()
                .Expect(t => t.DisplayFor(Arg<ElementRequest>.Is.Anything))
                .Return(new HtmlTag("span"));
        }

        [Test]
        public void should_populate_instance()
        {
            MockFor<IModelPopulator>()
                .Expect(p => p.PopulateInstance(_context.Instance, _context.SimpleProperties()))
                .IgnoreArguments();

            ClassUnderTest
                .Enrich(_context, _model);

            VerifyCallsFor<IModelPopulator>();
        }

        [Test]
        public void should_build_examples_for_simple_properties()
        {
            ClassUnderTest
                .Enrich(_context, _model);

            _model
                .Examples
                .ShouldHaveCount(1);
        }
    }
}