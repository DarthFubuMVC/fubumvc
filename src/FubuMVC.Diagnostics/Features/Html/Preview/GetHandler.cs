using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration;
using FubuMVC.Core.UI.Tags;
using HtmlTags;
using Microsoft.Practices.ServiceLocation;

namespace FubuMVC.Diagnostics.Features.Html.Preview
{
    // THAR BE DRAGONS! This is mostly ripped from the baseline diagnostics and needs some SERIOUS testing
    public class GetHandler
    {
        private readonly BehaviorGraph _behaviorGraph;
        private readonly IServiceLocator _services;

        public GetHandler(BehaviorGraph behaviorGraph, IServiceLocator services)
        {
            _behaviorGraph = behaviorGraph;
            _services = services;
        }

        public HtmlConventionsPreviewViewModel Execute(HtmlConventionsPreviewRequestModel request)
        {
            var modelPath = request.OutputModel;
            var propertyPath = new List<string>(modelPath.Split('-'));
            var displayPath = propertyPath.Join(".");
            var rootModelTypeName = propertyPath[0];
            propertyPath.RemoveAt(0);

            Type scannedModelType = getTypeFromName(rootModelTypeName);
            var scannedModelInstance = createInstance(scannedModelType);
            var tagGeneratorType = typeof(TagGenerator<>).MakeGenericType(scannedModelType);
            var tagGenerator = (ITagGenerator)_services.GetInstance(tagGeneratorType);
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

            var links = new List<PropertyLink>();
            propertiesToLink
                .Each(prop =>
                          {
                              var path = "{0}-{1}".ToFormat(modelPath, prop.Name);
                              var type = prop.PropertyType;
                              if (type.IsInterface || type.IsAbstract || type.IsGenericType
                                  || type.GetConstructor(new Type[0]) == null)
                              {
                                  path = "";
                              }


                              links.Add(new PropertyLink
                                            {
                                                Path = path,
                                                Source = getPropertySourceCode(prop)
                                            });
                          });

            // show examples
            populateInstance(scannedModelInstance, propertiesToShow);
            var examples = new List<PropertyExample>();
            propertiesToShow
                .Each(prop =>
                          {
                              var property = propertyChainParts.Count > 0 ? 
                                  (Accessor)new PropertyChain(propertyChainParts.Concat(new[] { prop }).Select(x => new PropertyValueGetter(x)).ToArray()) :
                                    new SingleProperty(prop);

                              var propertyExpression = "x => x." + property.PropertyNames.Join(".");
                              var propertySource = getPropertySourceCode(prop);

                              var propExamples = new List<Example>();
                              propExamples.Add(createExample(tagGenerator.LabelFor(tagGenerator.GetRequest(property)), "LabelFor({0})".ToFormat(propertyExpression)));
                              propExamples.Add(createExample(tagGenerator.DisplayFor(tagGenerator.GetRequest(property)), "DisplayFor({0})".ToFormat(propertyExpression)));
                              propExamples.Add(createExample(tagGenerator.InputFor(tagGenerator.GetRequest(property)), "InputFor({0})".ToFormat(propertyExpression)));

                              var propExample = new PropertyExample
                                                    {
                                                        Source = propertySource,
                                                        Examples = propExamples
                                                    };
                              examples.Add(propExample);
                          });

            return new HtmlConventionsPreviewViewModel
                       {
                           Type = displayPath,
                           Links = links,
                           Examples = examples
                       };
        }

        private static Example createExample(HtmlTag htmlTag, string expression)
        {
            return new Example
                       {
                           Expression = expression,
                           Source = htmlTag.ToString().HtmlEncode(),
                           Rendered = htmlTag
                       };
        }

        private static string getPropertySourceCode(PropertyInfo propertyInfo)
        {
            return string.Format("{0} {1} {2} {{ get; set; }}",
                                 propertyInfo.GetGetMethod().IsPublic ? "public" : "internal",
                                 propertyInfo.PropertyType.Name,
                                 propertyInfo.Name);
        }

        private Type getTypeFromName(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;

            return _behaviorGraph.Behaviors
                .Select(x => x.ActionOutputType())
                .FirstOrDefault(x => x != null && x.FullName == typeName);
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
            setMethod.Invoke(instance, new[] { propertyValue });
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
    }
}