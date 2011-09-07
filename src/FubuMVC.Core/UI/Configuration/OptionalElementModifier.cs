using HtmlTags;

namespace FubuMVC.Core.UI.Configuration
{
    public abstract class OptionalElementModifier : IElementModifier
    {
        public abstract bool Matches(AccessorDef def);

        public abstract void Build(ElementRequest request, HtmlTag tag);

        public TagModifier CreateModifier(AccessorDef accessorDef)
        {
            if (!Matches(accessorDef)) return (r, t) => { };
            return Build;
        }
    }
}