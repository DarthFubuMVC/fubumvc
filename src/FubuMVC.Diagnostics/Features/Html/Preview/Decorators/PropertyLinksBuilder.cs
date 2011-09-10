using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Diagnostics.Features.Html.Preview.Decorators
{
    public class PropertyLinksBuilder : IPreviewModelDecorator
    {
        private readonly IPropertySourceGenerator _sourceGenerator;

        public PropertyLinksBuilder(IPropertySourceGenerator sourceGenerator)
        {
            _sourceGenerator = sourceGenerator;
        }

        public void Enrich(HtmlConventionsPreviewContext context, HtmlConventionsPreviewViewModel model)
        {
            var links = new List<PropertyLink>();
            context
                .NonConvertibleProperties()
                .Each(prop =>
                {
                    var path = "{0}-{1}".ToFormat(context.Path, prop.Name);
                    var type = prop.PropertyType;
                    if (type.IsInterface || type.IsAbstract || type.IsGenericType
                        || type.GetConstructor(new Type[0]) == null)
                    {
                        path = "";
                    }


                    links.Add(new PropertyLink
                    {
                        Path = path,
                        Source = _sourceGenerator.SourceFor(prop)
                    });
                });

            model.Links = links;
        }
    }
}