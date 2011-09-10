using System.Reflection;
using FubuMVC.Diagnostics.Features.Html.Preview;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Features.Html
{
    [TestFixture]
    public class HtmlConventionsPreviewContextTester
    {
        private HtmlConventionsPreviewContext _context;

        [SetUp]
        public void setup()
        {
            _context = ObjectMother.BasicPreviewContext();
        }

        [Test]
        public void should_select_public_properties()
        {
            _context
                .Properties
                .ShouldHaveCount(3);
        }

        [Test]
        public void should_select_properties_whose_types_cannot_be_converted_to_strings()
        {
            _context
                .NonConvertibleProperties()
                .ShouldHaveCount(2);
        }

        [Test]
        public void should_select_ancillary_properties()
        {
            _context
                .SimpleProperties()
                .ShouldHaveCount(1);
        }
    }
}