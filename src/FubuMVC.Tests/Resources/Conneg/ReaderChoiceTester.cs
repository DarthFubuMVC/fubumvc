using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Resources.Conneg
{
    [TestFixture]
    public class ReaderChoiceTester
    {
        [Test]
        public void get_the_description_if_the_reader_is_null()
        {
            var mimeType = new CurrentMimeType("text/json", null);

            var choice = new ReaderChoice(mimeType, null);

            var description = Description.For(choice);

            description.Title.ShouldEqual("Unable to select a reader for content-type 'text/json'");
        }

        [Test]
        public void get_the_description_if_the_reader_is_not_null()
        {
            var mimeType = new CurrentMimeType("text/json", null);

            var choice = new ReaderChoice(mimeType, new ClassWithTitle());

            var description = Description.For(choice);

            description.Title.ShouldEqual("Selected reader 'Some title' for content-type 'text/json'");
        }
    }

    [Title("Some title")]
    public class ClassWithTitle
    {
        
    }
}