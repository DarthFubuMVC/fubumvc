using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.UI.Configuration;
using HtmlTags;

namespace FubuMVC.UI.Tags
{
    public class TagFactory
    {
        private readonly Cache<AccessorDef, Func<ElementRequest, HtmlTag>> _creators =
            new Cache<AccessorDef, Func<ElementRequest, HtmlTag>>();

        private readonly IList<IElementModifier> _modifiers = new List<IElementModifier>();
        private readonly IList<IElementBuilder> _sources = new List<IElementBuilder>();

        public TagFactory()
        {
            _creators.OnMissing = resolveCreator;
        }

        public void AddModifier(IElementModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void AddBuilder(IElementBuilder builder)
        {
            _sources.Add(builder);
        }

        private Func<ElementRequest, HtmlTag> resolveCreator(AccessorDef accessorDef)
        {
            TagBuilder initialCreator = _sources.FirstValue(x => x.CreateInitial(accessorDef));
            if (initialCreator == null)
            {
                throw new FubuException(3000, "Html Conventions have no tag builder for {0}.{1}", accessorDef.ModelType.FullName, accessorDef.Accessor.Name);
            }

            TagModifier[] modifiers =
                _modifiers.Select(x => x.CreateModifier(accessorDef)).Where(x => x != null).ToArray();

            return request =>
            {
                HtmlTag tag = initialCreator(request);
                modifiers.Each(x => x(request, tag));

                return tag;
            };
        }

        public HtmlTag Build(ElementRequest request)
        {
            return _creators[request.ToAccessorDef()](request);
        }

        public void Merge(TagFactory factory)
        {
            _sources.AddRange(factory._sources);
            _modifiers.AddRange(factory._modifiers);
        }
    }
}