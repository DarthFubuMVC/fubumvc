using System.Linq;
using FubuCore;
using FubuMVC.Diagnostics.Features.Html;
using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuMVC.Diagnostics.Features.Html.Preview.Decorators;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Features.Html.Decorators
{
    [TestFixture]
    public class PropertyLinksBuilderTester
    {
        private PropertyLinksBuilder _builder;
        private HtmlConventionsPreviewViewModel _model;
        private HtmlConventionsPreviewContext _context;

        [SetUp]
        public void setup()
        {
            _builder = new PropertyLinksBuilder(new PropertySourceGenerator());
            _model = new HtmlConventionsPreviewViewModel();
            _context = ObjectMother.BasicPreviewContext();
        }

        [Test]
        public void should_exclude_interfaces_abstracts_and_generics()
        {
            _builder.Enrich(_context, _model);
            _model
                .Links
                .Where(l => l.Path.IsNotEmpty())
                .ShouldHaveCount(1); // SampleContextModel.Child
        }
    }
}