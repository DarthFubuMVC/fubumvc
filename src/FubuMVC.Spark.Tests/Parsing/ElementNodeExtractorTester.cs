using System.Text;
using FubuCore;
using FubuMVC.Spark.Parsing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Parsing
{
    public class ElementNodeExtractorTester : InteractionContext<ElementNodeExtractor>
    {
        private const string MasterTemplate = @"<use master=""{0}"" />";
        private const string ViewdataTemplate = @"<viewdata model=""{0}"" />";
        private const string ContentTemplate = "<div>{0}</div>";

        private readonly StringBuilder _builder = new StringBuilder();
        protected override void beforeEach()
        {
            _builder.Clear();
        }

        [Test]
        public void it_can_extract_use_statement()
        {
            _builder.AppendLine(MasterTemplate.ToFormat("fubu"));
            _builder.AppendLine(ContentTemplate.ToFormat("FubuMVC rules"));

            ClassUnderTest.ExtractByName(_builder.ToString(), "use")
                .ShouldHaveCount(1)
                .ShouldContain(e => e.AttributeByName("master") == "fubu");
        }

        [Test]
        public void it_can_extract_viewdata_statement()
        {
            _builder.AppendLine(MasterTemplate.ToFormat("fubu"));
            _builder.AppendLine(ViewdataTemplate.ToFormat("a.b.c"));
            _builder.AppendLine(ContentTemplate.ToFormat("content"));

            ClassUnderTest.ExtractByName(_builder.ToString(), "viewdata")
                .ShouldHaveCount(1)
                .ShouldContain(e => e.AttributeByName("model") == "a.b.c");
        }
    }
}