using System.Text;
using FubuCore;
using FubuMVC.Spark.Tokenization.Parsing;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Parsing
{
    public class ElementNodeExtractorTester : InteractionContext<ElementNodeExtractor>
    {
        private readonly StringBuilder _builder = new StringBuilder();
        protected override void beforeEach()
        {
            _builder.Clear();
        }

        [Test]
        public void it_can_extract_use_statement()
        {
            _builder.AppendLine(Templates.UseMaster.ToFormat("fubu"));
            _builder.AppendLine(Templates.Content.ToFormat("FubuMVC rules"));

            ClassUnderTest.ExtractByName(_builder.ToString(), "use")
                .ShouldHaveCount(1)
                .ShouldContain(e => e.AttributeByName("master") == "fubu");
        }

        [Test]
        public void it_can_extract_viewdata_statement()
        {
            _builder.AppendLine(Templates.UseMaster.ToFormat("fubu"));
            _builder.AppendLine(Templates.ViewdataModel.ToFormat("a.b.c"));
            _builder.AppendLine(Templates.Content.ToFormat("content"));

            ClassUnderTest.ExtractByName(_builder.ToString(), "viewdata")
                .ShouldHaveCount(1)
                .ShouldContain(e => e.AttributeByName("model") == "a.b.c");
        }
    }
}