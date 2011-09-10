using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Diagnostics.Features.Html;
using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuMVC.Diagnostics.Features.Html.Preview.Decorators;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Features.Html.Decorators
{
    [TestFixture]
    public class DisplayPathBuilderTester
    {
        private DisplayPathBuilder _builder;
        private HtmlConventionsPreviewViewModel _model;

        [SetUp]
        public void setup()
        {
            _builder = new DisplayPathBuilder();
            _model = new HtmlConventionsPreviewViewModel();
        }

        [Test]
        public void should_display_model_type_for_initial_requests()
        {
            var context = ObjectMother.BasicPreviewContext();
            _builder.Enrich(context, _model);

            _model
                .Type
                .ShouldEqual(context.ModelType.FullName);
        }

        [Test]
        public void should_build_property_paths_with_periods()
        {
            var prop = ReflectionHelper.GetProperty<SampleContextModel>(m => m.Child);
            var context = new HtmlConventionsPreviewContext(typeof(SampleContextModel).FullName, typeof (SampleContextModel),
                                                            new SampleContextModel(), new PropertyInfo[] { prop });

            _builder.Enrich(context, _model);

            _model
                .Type
                .ShouldEqual("{0}.{1}".ToFormat(context.ModelType.FullName, prop.Name));
        }
    }
}