using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Diagnostics.HtmlWriting.Columns;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.UI.Tags;
using FubuMVC.Core.Urls;
using HtmlTags;

using System.Linq;

namespace FubuMVC.Core.UI.Diagnostics
{
    [FubuDiagnostics("Html Conventions")]
    public class ExampleHtmlWriter
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly IUrlRegistry _urlRegistry;
        private readonly BehaviorGraph _behaviorGraph;
        private readonly ICurrentHttpRequest _httpRequest;
        private readonly string _examplePageUrl;
        private const string UrlPattern = "_fubu/html/example";

        public ExampleHtmlWriter(IServiceLocator serviceLocator, IUrlRegistry urlRegistry, BehaviorGraph behaviorGraph, ICurrentHttpRequest httpRequest)
        {
            _serviceLocator = serviceLocator;
            _urlRegistry = urlRegistry;
            _behaviorGraph = behaviorGraph;
            _httpRequest = httpRequest;

            _examplePageUrl = httpRequest.ToFullUrl(UrlPattern);
        }

        [UrlPattern("_fubu/html"), Description("Demonstrates effects of current HTML conventions")]
        public HtmlDocument Html()
        {
            var tags = new List<HtmlTag> { showIntro() };

            IEnumerable<Type> ignoredModels = new Type[0];  // TODO -- come back to this?
            var table = BehaviorGraphWriter.WriteBehaviorChainTable(_behaviorGraph.Behaviors
                .Where(b => b.HasOutput() && !b.ResourceType().IsSimple())
                .Where(b => b.Outputs.Select(o => o.GetType()).Except(ignoredModels).Any() )
                .OrderBy(b => b.GetRoutePattern()),
                new RouteColumn(_httpRequest),
                new OutputModelColumn(_urlRegistry),
                new OutputColumn());
            tags.Add(table);
            var doc = DiagnosticHtml.BuildDocument(_urlRegistry, "FubuMVC.UI Examples", tags.ToArray());
            doc.AddStyle(DiagnosticHtml.GetResourceText(GetType(), "examples.css"));
            return doc;
        }

        [UrlPattern(UrlPattern)]
        public HtmlDocument Example(ExampleHtmlRequest exampleHtmlRequest)
        {
            var modelPath = exampleHtmlRequest.Model ?? typeof(ExampleViewModel).FullName + "-Person";
            var tags = new List<HtmlTag>();
            var propertyPath = new List<string>(modelPath.Split('-'));
            var rootModelTypeName = propertyPath[0];
            tags.Add(new HtmlTag("h3").AddClass("viewmodel").Text(propertyPath.Join(".")));
            propertyPath.RemoveAt(0);

            Type scannedModelType = getTypeFromName(rootModelTypeName);
            var scannedModelInstance = createInstance(scannedModelType);
            var tagGeneratorType = typeof(TagGenerator<>).MakeGenericType(scannedModelType);
            var tagGenerator = (ITagGenerator)_serviceLocator.GetInstance(tagGeneratorType);
            tagGenerator.SetModel(scannedModelInstance);

            var propertyChainParts = new List<PropertyInfo>();

            while (propertyPath.Count > 0)
            {
                var parentPropertyInfo = scannedModelType.GetProperty(propertyPath[0]);
                propertyChainParts.Add(parentPropertyInfo);
                var currentModelType = parentPropertyInfo.PropertyType;
                var currentModel = createInstance(currentModelType);
                setProperty(parentPropertyInfo, scannedModelInstance, currentModel);

                scannedModelType = currentModelType;
                scannedModelInstance = currentModel;
                propertyPath.RemoveAt(0);
            }


            var modelProperties = scannedModelType.GetProperties();
            var propertiesToLink = modelProperties.Where(p => !TypeDescriptor.GetConverter(p.PropertyType).CanConvertFrom(typeof(string)));
            var propertiesToShow = modelProperties.Except(propertiesToLink);

            // show links to deeper properties
            var linkList = new HtmlTag("ul").AddClass("subproperties");
            foreach (var propertyInfo in propertiesToLink)
            {
                var url = _urlRegistry.UrlFor(new ExampleHtmlRequest {Model = modelPath + "-" + propertyInfo.Name});
                var linkTag = new LinkTag("", url);
                linkTag.Append(new HtmlTag("code").Text(getPropertySourceCode(propertyInfo)));
                var listItem = new HtmlTag("li").Append(linkTag);
                linkList.Append(listItem);
            }
            if (linkList.Children.Count > 0) tags.Add(linkList);

            // show examples
            populateInstance(scannedModelInstance, propertiesToShow);
            foreach (var propertyInfo in propertiesToShow)
            {
                var property = propertyChainParts.Count > 0 ?
                    (Accessor) new PropertyChain(propertyChainParts.Concat(new[]{propertyInfo}).Select(x => new PropertyValueGetter(x)).ToArray()) :
                    new SingleProperty(propertyInfo);
                
                var propertyExpression = "x => x." + property.PropertyNames.Join(".");
                
                var propertySource = getPropertySourceCode(propertyInfo);
                
                var example = new HtmlTag("div").AddClass("example");
                example.Append(new HtmlTag("code").AddClass("property").Text(propertySource));
                example.Append(createExample(tagGenerator.LabelFor(tagGenerator.GetRequest(property)), "LabelFor({0})".ToFormat(propertyExpression)));
                example.Append(createExample(tagGenerator.DisplayFor(tagGenerator.GetRequest(property)), "DisplayFor({0})".ToFormat(propertyExpression)));
                example.Append(createExample(tagGenerator.InputFor(tagGenerator.GetRequest(property)), "InputFor({0})".ToFormat(propertyExpression)));
                tags.Add(example);
            }

            var doc = DiagnosticHtml.BuildDocument(_urlRegistry, "FubuMVC.UI Examples", tags.ToArray());
            doc.AddStyle(DiagnosticHtml.GetResourceText(GetType(), "examples.css"));
            return doc;
        }

        private Type getTypeFromName(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;

            return _behaviorGraph.Behaviors
                .Select(x => x.ResourceType())
                .FirstOrDefault(x => x != null && x.FullName == typeName);
        }

        private static string getPropertySourceCode(PropertyInfo propertyInfo)
        {
            return string.Format("{0} {1} {2} {{ get; set; }}",
                                 propertyInfo.GetGetMethod().IsPublic ? "public" : "internal",
                                 propertyInfo.PropertyType.Name,
                                 propertyInfo.Name);
        }

        private static object createInstance(Type modelType)
        {
            return Activator.CreateInstance(modelType);
        }

        private static void populateInstance(object instance, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var propertyValue = exampleValue(property.PropertyType);
                if (propertyValue != null)
                {
                    setProperty(property, instance, propertyValue);
                }
            }
        }

        private static void setProperty(PropertyInfo property, object instance, object propertyValue)
        {
            var setMethod = property.GetSetMethod();
            setMethod.Invoke(instance, new[] {propertyValue});
        }


        private static object exampleValue(Type propertyType)
        {
            if (propertyType == typeof(string)) return "Hello World";
            if (propertyType == typeof(char)) return 'F';
            if (propertyType == typeof(bool)) return true;
            if (propertyType.IsIntegerBased()) return 42;
            if (propertyType.IsFloatingPoint()) return 3.14;
            if (propertyType.IsDateTime()) return DateTime.Now;
            return null;
        }


        private static HtmlTag createExample(HtmlTag htmlTag, string methodCall)
        {
            var example = new HtmlTag("fieldset").AddClass("tag");
            example.Add("legend").Text(methodCall);
            example.Append(new HtmlTag("code").AddClass("source").Text(htmlTag.ToString()));
            example.Append(new HtmlTag("div").AddClass("rendered").Append(htmlTag));
            return example;
        }

        private HtmlTag showIntro()
        {
            var container = new HtmlTag("div").AddClass("intro");
            container.Append(
            new HtmlTag("p").Text(@"
These pages demonstrate the output that is rendered when using the FubuMVC.UI conventional HTML generators (InputFor/DisplayFor/LabelFor).
To alter how the tags are generated, create your own class that derives from HtmlConventionRegistry, and declare it in your FubuRegistry using:"
));
            container.Append(new HtmlTag("pre").Text("this.HtmlConvention<MyHtmlConventionRegistry>();"));
            container.Append(
            new HtmlTag("p").Text(@"
To alter how a property value is converted to a string value, use the StringConversions() extension method in your FubuRegistry. For example:"
));

            container.Append(new HtmlTag("pre").Text(@"
this.StringConversions(x =>
{
   x.IfIsType<DateTime>().ConvertBy(date => date.ToShortDateString());
});
"));
            container.Append(new HtmlTag("p").Text(@"You can see the conventions applied by selecting one of your view models below, or applied to ").Append(new LinkTag("the built-in example model.", _examplePageUrl)));
            return container;
        }
    }

    internal class OutputModelColumn : IColumn
    {
        private readonly IUrlRegistry _urlRegistry;

        public OutputModelColumn(IUrlRegistry urlRegistry)
        {
            _urlRegistry = urlRegistry;
        }

        public string Header()
        {
            return "View Model";
        }

        public void WriteBody(BehaviorChain chain, HtmlTag row, HtmlTag cell)
        {
            var outputType = chain.ResourceType();
            var url = _urlRegistry.UrlFor(new ExampleHtmlRequest {Model = outputType.FullName});
            cell.Append(new LinkTag(outputType.Name, url));
        }

        public string Text(BehaviorChain chain)
        {
            return chain.ResourceType().Name;
        }
    }

    public class ExampleHtmlRequest
    {
        [QueryString]
        public string Model { get; set; }
    }

    internal class ExampleViewModel
    {
        public Person Person { get; set; }
    }

    internal class Person
    {
        public string Name { get; set; }
        public Address HomeAddress { get; set; }
        public int Age { get; set; }
        public bool Married { get; set; }
        public DateTime Birthday { get; set; }
    }

    internal class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
    }
}