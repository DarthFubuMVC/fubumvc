using System.Collections.Generic;
using NUnit.Framework;

namespace FubuValidation.Tests
{
    [TestFixture]
    public class TemplateParserTester
    {
        [Test]
        public void should_replace_variables()
        {
            var template = "this {is} a {test} template with {a} few {variables}";
            var substitutions = new Dictionary<string, string>
                                    {
                                        {"is", "is"},
                                        {"test", "replaced"},
                                        {"a", "more than a"},
                                        {"variables", "witty tricks."}
                                    };

            TemplateParser
                .Parse(template, substitutions)
                .ShouldEqual("this is a replaced template with more than a few witty tricks.");
        }
    }
}