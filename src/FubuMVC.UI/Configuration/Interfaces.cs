using System;
using FubuCore.Reflection;
using HtmlTags;

namespace FubuMVC.UI.Configuration
{
    public delegate HtmlTag TagBuilder(ElementRequest request);

    public delegate void TagModifier(ElementRequest request, HtmlTag tag);

    public interface IElementBuilder
    {
        TagBuilder CreateInitial(AccessorDef accessorDef);
    }

    public abstract class ElementBuilder : IElementBuilder
    {
        public TagBuilder CreateInitial(AccessorDef accessorDef)
        {
            if (matches(accessorDef)) return Build;

            return null;
        }

        protected abstract bool matches(AccessorDef def);

        public abstract HtmlTag Build(ElementRequest request);
    }

    public interface IElementModifier
    {
        TagModifier CreateModifier(AccessorDef accessorDef);
    }

    public interface IElementNamingConvention
    {
        string GetName(Type modelType, Accessor accessor);
    }

    public class DefaultElementNamingConvention : IElementNamingConvention
    {
        public string GetName(Type modelType, Accessor accessor)
        {
            return accessor.Name;
        }
    }
}