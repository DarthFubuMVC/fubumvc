using FubuCore.Descriptions;
using FubuMVC.Core.Resources.Conneg;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Resources.Conneg
{
    [TestFixture]
    public class ReaderChoiceTester
    {
        [Test]
        public void get_the_description_if_the_reader_is_null()
        {
            var choice = new ReaderChoice("text/json", null);

            var description = Description.For(choice);

            description.Title.ShouldBe("Unable to select a reader for content-type 'text/json'");
        }

        [Test]
        public void get_the_description_if_the_reader_is_not_null()
        {
            var choice = new ReaderChoice("text/json", new ClassWithTitle());

            var description = Description.For(choice);

            description.Title.ShouldBe("Selected reader 'Some title' for content-type 'text/json'");
        }
    }

    [Title("Some title")]
    public class ClassWithTitle
    {
    }
}