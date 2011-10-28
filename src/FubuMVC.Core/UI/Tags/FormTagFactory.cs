using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;

namespace FubuMVC.Core.UI.Tags
{
    public class FormTagFactory
    {
        private readonly Cache<FormDef, Func<FormElementRequest, FormTag>> _creators =
            new Cache<FormDef, Func<FormElementRequest, FormTag>>();

        private readonly IList<IFormElementModifier> _modifiers = new List<IFormElementModifier>();

        private readonly Func<FormElementRequest, FormTag> _builder;

        public FormTagFactory()
        {
            _builder = req =>
            {
                var tag = (FormTag)new FormTag(req.Url).Id(string.Empty); 
                
                if (req.InBound)
                {
                    var method = req.TargetChain.Route.AllowedHttpMethods.OrderBy(x => x != "GET").First();
                   tag.Method(method);
                }

                return tag;
            };
            _creators.OnMissing = resolveCreator;
        }

        public void AddModifier(IFormElementModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        private Func<FormElementRequest, FormTag> resolveCreator(FormDef formType)
        {
            FormTagModifier[] modifiers =
                _modifiers.Select(x => x.CreateModifier(formType)).Where(x => x != null).ToArray();

            return request =>
            {
                var tag = buildTag(request);

                modifiers.Each(x => x(request, tag));

                return tag;
            };
        }

        private FormTag buildTag(FormElementRequest request)
        {
            return _builder(request);
        }

        public FormTag Build(FormElementRequest request)
        {

            return _creators[request.ToFormDef()](request);
        }

        public void Merge(FormTagFactory factory)
        {
            _modifiers.AddRange(factory._modifiers);
        }
    }

}