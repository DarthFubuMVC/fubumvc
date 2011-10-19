using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.Urls;
using HtmlTags;
using System;

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

            AfterFormCreate.If(x =>
                                   {
                                       if (x.ModelType.IsGenericType == false)
                                       {
                                           return false;
                                       }
                                       return x.ModelType.GetGenericTypeDefinition() == typeof(Expression<>);
                                   })
            .BuildBy(req =>
                         {
                             // this could be less ugly with use of the dynamic keyword, however fubumvc does not
                             // have the Microsoft.CSharp assembly
                             // I've used Single instead of first, so it'll explode if IUrlRegistry's method sigs will change :)

                             var expression = (LambdaExpression)req.Model;
                             var method = typeof(IUrlRegistry).GetMethods().Single(x => x.Name == "UrlFor" && x.IsGenericMethod);
                             var urlRegistry = req.Get<IUrlRegistry>();
                             var genericMethod = method.MakeGenericMethod(expression.Parameters[0].Type);
                             var url = (string)genericMethod.Invoke(urlRegistry, new[] { expression });
                             return new FormTag(url);
                         });

            AfterFormCreate.Always.BuildBy(req =>
            {
                var urlRegistry = req.Get<IUrlRegistry>();
                var url = urlRegistry.UrlFor(req.Model);
                return new FormTag(url);
            });
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