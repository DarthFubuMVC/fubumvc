using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;

namespace FubuMVC.Core.UI.Tags
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

        public void InsertFirstBuilder(IElementBuilder builder)
        {
            _sources.Insert(0, builder);
        }

        private Func<ElementRequest, HtmlTag> resolveCreator(AccessorDef accessorDef)
        {
            // TODO -- going to be largely rewritten soon-ish
            //TagBuilder initialCreator = _sources.FirstValue(x => x.CreateInitial(accessorDef));
            //if (initialCreator == null)
            //{
            //    throw new FubuException(3000, "Html Conventions have no tag builder for {0}.{1}", accessorDef.ModelType.FullName, accessorDef.Accessor.Name);
            //}

            TagModifier[] modifiers =
                _modifiers.Select(x => x.CreateModifier(accessorDef)).Where(x => x != null).ToArray();

            return request =>
            {
                //HtmlTag tag = initialCreator(request);
                var tag = buildTag(request);
                
                modifiers.Each(x => x(request, tag));

                return tag;
            };
        }

        private HtmlTag buildTag(ElementRequest request)
        {
            var accessorDef = request.ToAccessorDef();

            foreach (var builder in _sources)
            {
                var creator = builder.CreateInitial(accessorDef);
                if (creator == null) continue;

                var tag = creator(request);
                if (tag != null) return tag;
            }

            throw new FubuException(3000, "Html Conventions have no tag builder for {0}.{1}", accessorDef.ModelType.FullName, accessorDef.Accessor.Name);
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