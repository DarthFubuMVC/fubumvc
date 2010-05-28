using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Urls;
using FubuMVC.UI.Tags;
using HtmlTags;

namespace FubuMVC.UI.Diagnostics
{
    internal class ExampleController
    {
        private readonly TagGenerator<ExampleViewModel> _tagGenerator;
        private readonly IUrlRegistry _urlRegistry;

        public ExampleController(TagGenerator<ExampleViewModel> tagGenerator, IUrlRegistry urlRegistry)
        {
            _tagGenerator = tagGenerator;
            _urlRegistry = urlRegistry;
        }

        [UrlPattern("_fubu/html")]
        public string Default()
        {
            _tagGenerator.Model = new ExampleViewModel
                {
                    Person = new Person
                        {
                            Name = "John Doe",
                            Age = 42,
                            Birthday = DateTime.Today.AddYears(42),
                            Married = true
                        }
                };

            var doc = DiagnosticHtml.BuildDocument(_urlRegistry, "FubuMVC.UI Examples",
                                                        showProperty(x => x.Person.Name),
                                                        showProperty(x => x.Person.Age),
                                                        showProperty(x => x.Person.Birthday),
                                                        showProperty(x => x.Person.Married)
                );
            doc.AddStyle(DiagnosticHtml.GetResourceText(GetType(), "examples.css"));
            return doc.ToString();
        }

        private HtmlTag showProperty(Expression<Func<ExampleViewModel, object>> propertyExpression)
        {
            var property = ReflectionHelper.GetAccessor(propertyExpression).InnerProperty;
            var propertySource = string.Format("{0} {1} {2} {{ get; set; }}",
                property.GetGetMethod().IsPublic ? "public" : "internal",
                property.PropertyType.Name,
                property.Name);
            var expressionText = getExpressionText(propertyExpression);
            var example = new HtmlTag("div").AddClass("example");
            example.AddChildren(new HtmlTag("code").AddClass("property").Text(propertySource));
            example.AddChildren(createExample(_tagGenerator.LabelFor(propertyExpression), "LabelFor(" + expressionText + ")"));
            example.AddChildren(createExample(_tagGenerator.DisplayFor(propertyExpression), "DisplayFor(" + expressionText + ")"));
            example.AddChildren(createExample(_tagGenerator.InputFor(propertyExpression), "InputFor(" + expressionText + ")"));
            return example;
        }

        private static string getExpressionText(Expression<Func<ExampleViewModel, object>> propertyExpression)
        {
            var expressionText = propertyExpression.ToString();
            // hack to get rid of convert calls. probably not reliable
            const string convert = "Convert(";
            if (expressionText.Contains(convert))
            {
                expressionText = expressionText.Replace(convert, "").TrimEnd(')');
            }
            return expressionText;
        }

        private static HtmlTag createExample(HtmlTag htmlTag, string methodCall)
        {
            var example = new HtmlTag("fieldset").AddClass("tag");
            example.Add("legend").Text(methodCall);
            example.AddChildren(new HtmlTag("code").AddClass("source").Text(htmlTag.ToString()));
            example.AddChildren(new HtmlTag("div").AddClass("rendered").AddChildren(htmlTag));
            return example;
        }
    }

    internal class ExampleViewModel
    {
        public Person Person { get; set; }
    }

    internal class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Married { get; set; }
        public DateTime Birthday { get; set; }
    }
}