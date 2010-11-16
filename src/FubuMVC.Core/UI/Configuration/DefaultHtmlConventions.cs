using System.Linq;
using System.Text.RegularExpressions;
using FubuMVC.Core.UI.Tags;
using HtmlTags;

namespace FubuMVC.Core.UI.Configuration
{
    public class DefaultHtmlConventions : HtmlConventionRegistry
    {
        public DefaultHtmlConventions()
        {
            Editors.IfPropertyIs<bool>().BuildBy(TagActionExpression.BuildCheckbox);

            // Relax, this is just the default fall through action
            Editors.Always.BuildBy(TagActionExpression.BuildTextbox);

            Editors.Always.Modify(AddElementName);
            Displays.Always.BuildBy(req => new HtmlTag("span").Text(req.StringValue()));

            Labels.Always.BuildBy(req => new HtmlTag("label").Text(BreakUpCamelCase(req.Accessor.FieldName)));

            BeforePartial.Always.BuildBy(req => new NoTag());
            AfterPartial.Always.BuildBy(req => new NoTag());
            BeforeEachOfPartial.Always.BuildBy((req, index, count) => new NoTag());
            AfterEachOfPartial.Always.BuildBy((req, index, count) => new NoTag());
        }

        public static string BreakUpCamelCase(string fieldName)
        {
            var patterns = new[]
                {
                    "([a-z])([A-Z])",
                    "([0-9])([a-zA-Z])",
                    "([a-zA-Z])([0-9])"
                };
            var output = patterns.Aggregate(fieldName, 
                (current, pattern) => Regex.Replace(current, pattern, "$1 $2", RegexOptions.IgnorePatternWhitespace));
            return output.Replace('_', ' ');
        }

        public static void AddElementName(ElementRequest request, HtmlTag tag)
        {
            if (tag.IsInputElement() && !tag.HasAttr("name"))
            {
                tag.Attr("name", request.ElementId);
            }
        }
    }
}