using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class SelectTagTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void selected_by_value()
        {
            var tag = new SelectTag();
            tag.Option("a", "1");
            tag.Option("b", "2");
            tag.Option("c", "3");

            tag.SelectByValue("2");

            tag.Children[0].HasAttr("selected").ShouldBeFalse();
            tag.Children[1].Attr("selected").ShouldEqual("selected");
            tag.Children[2].HasAttr("selected").ShouldBeFalse();


        }
    }
}