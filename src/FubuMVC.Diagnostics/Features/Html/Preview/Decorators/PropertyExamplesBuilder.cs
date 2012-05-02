using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using HtmlTags;

namespace FubuMVC.Diagnostics.Features.Html.Preview.Decorators
{
    using FubuMVC.Core;

    public class PropertyExamplesBuilder : IPreviewModelDecorator
    {
        private readonly IModelPopulator _populator;
        private readonly IPropertySourceGenerator _sourceGenerator;
        private readonly ITagGeneratorFactory _generatorFactory;

        public PropertyExamplesBuilder(IModelPopulator populator, IPropertySourceGenerator sourceGenerator, ITagGeneratorFactory generatorFactory)
        {
            _populator = populator;
            _sourceGenerator = sourceGenerator;
            _generatorFactory = generatorFactory;
        }

        public void Enrich(HtmlConventionsPreviewContext context, HtmlConventionsPreviewViewModel model)
        {
            var examples = new List<PropertyExample>();
            var tagGenerator = _generatorFactory.GeneratorFor(context.ModelType);
            
            tagGenerator.SetModel(context.Instance);
            _populator.PopulateInstance(context.Instance, context.SimpleProperties());
            
            context
                .SimpleProperties()
                .Each(prop =>
                          {
                              Accessor property;
                              if(context.PropertyChain.Any())
                              {
                                  property = new PropertyChain(context
                                      .PropertyChain
                                      .Concat<PropertyInfo>(new[] {prop})
                                      .Select(x => new PropertyValueGetter(x))
                                      .ToArray());
                              }
                              else
                              {
                                  property = new SingleProperty(prop);
                              }

                              var propertyExpression = "x => x." + property.PropertyNames.Join(".");
                              var propertySource = _sourceGenerator.SourceFor(prop);

                              var propExamples = new List<Example>();
                              propExamples.Add(createExample(tagGenerator.LabelFor(tagGenerator.GetRequest(property)),
                                                             "LabelFor({0})".ToFormat(propertyExpression)));
                              propExamples.Add(createExample(
                                  tagGenerator.DisplayFor(tagGenerator.GetRequest(property)),
                                  "DisplayFor({0})".ToFormat(propertyExpression)));
                              propExamples.Add(createExample(tagGenerator.InputFor(tagGenerator.GetRequest(property)),
                                                             "InputFor({0})".ToFormat(propertyExpression)));

                              var propExample = new PropertyExample
                                                    {
                                                        Source = propertySource,
                                                        Examples = propExamples
                                                    };
                              examples.Add(propExample);
                          });

            model.Examples = examples;
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
    }
}