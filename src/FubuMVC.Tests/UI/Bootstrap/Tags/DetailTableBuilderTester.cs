using System.Linq;
using FubuCore;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Bootstrap.Tags;
using FubuTestingSupport;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Bootstrap.Tags
{
    [TestFixture]
    public class DetailTableBuilderTester
    {
        private FubuHtmlDocument theDocument;
        private DetailTableBuilder theBuilder;

        [SetUp]
        public void SetUp()
        {
            var services = new InMemoryServiceLocator();
            

            theDocument = new FubuHtmlDocument(services);
            theDocument.Push("div");

            theBuilder = new DetailTableBuilder(theDocument);
        }

        private void assertThatThereAreNoDetailRows()
        {
            theBuilder.DetailTag.TBody.Children.Any().ShouldBeFalse();
        }

        private HtmlTag lastRow
        {
            get
            {
                return theBuilder.DetailTag.TBody.FirstChild();
            }
        }

        [Test]
        public void adding_a_blank_string_value_adds_no_row()
        {
            string nullString = null;

            theBuilder.AddDetail("Something", nullString);
            assertThatThereAreNoDetailRows();

            theBuilder.AddDetail("Something", string.Empty);
            assertThatThereAreNoDetailRows();
        }

        [Test]
        public void adding_a_non_empty_string_adds_a_row()
        {
            theBuilder.AddDetail("Some Field", "Some Value");

            lastRow.ToString().ShouldEqual("<tr><th><span>Some Field</span></th><td>Some Value</td></tr>");
        }

        [Test]
        public void adding_an_empty_enumerable_of_strings_does_not_add_a_row()
        {
            theBuilder.AddDetail("Something", new string[0]);
            assertThatThereAreNoDetailRows();
        }

        [Test]
        public void adding_a_string_enumerable()
        {
            theBuilder.AddDetail("Some Field", new string[]{"a", "b", "c"});

            lastRow.ToString().ShouldEqual("<tr><th><span>Some Field</span></th><td>a, b, c</td></tr>");
        }
    }
}