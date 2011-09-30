using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Diagnostics.Features.Html.Preview.Decorators
{
    public class DisplayPathBuilder : IPreviewModelDecorator
    {
        public void Enrich(HtmlConventionsPreviewContext context, HtmlConventionsPreviewViewModel model)
        {
            var path = context.ModelType.FullName;
            if(context.PropertyChain.Any())
            {
                path = "{0}.{1}".ToFormat(path, context.PropertyChain.Select(p => p.Name).Join("."));
            }

            model.Type = context.ModelType.Name;
            model.Namespace = context.ModelType.Namespace;
            model.Assembly = context.ModelType.Assembly.GetName().Name;
        }
    }
}