using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;

namespace FubuMVC.Core.UI.Tags
{
    public class PartialTagFactory
    {
        private readonly Cache<AccessorDef, Func<ElementRequest, int, int, HtmlTag>> _creators =
            new Cache<AccessorDef, Func<ElementRequest, int, int, HtmlTag>>();

        private readonly IList<IPartialElementModifier> _modifiers = new List<IPartialElementModifier>();
        private readonly IList<IPartialElementBuilder> _sources = new List<IPartialElementBuilder>();

        public PartialTagFactory()
        {
            _creators.OnMissing = resolveCreator;
        }

        public void AddModifier(IPartialElementModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        public void AddBuilder(IPartialElementBuilder builder)
        {
            _sources.Add(builder);
        }

        private Func<ElementRequest, int, int, HtmlTag> resolveCreator(AccessorDef accessorDef)
        {
            EachPartialTagBuilder initialCreator = _sources.FirstValue(x => x.CreateInitial(accessorDef));
            if (initialCreator == null)
            {
                throw new FubuException(3001, "Html Conventions have no tag builder for partials for {0}.{1}", accessorDef.ModelType.FullName, accessorDef.Accessor.Name);
            }

            EachPartialTagModifier[] modifiers =
                _modifiers.Select(x => x.CreateModifier(accessorDef)).Where(x => x != null).ToArray();

            return (request, index, count) =>
            {
                HtmlTag tag = initialCreator(request, index, count);
                modifiers.Each(x => x(request, tag, index, count));

                return tag;
            };


        }

        public HtmlTag Build(ElementRequest request, int index, int count)
        {
            return _creators[request.ToAccessorDef()](request, index, count);
        }

        public void Merge(PartialTagFactory factory)
        {
            _sources.AddRange(factory._sources);
            _modifiers.AddRange(factory._modifiers);
        }
    }
}