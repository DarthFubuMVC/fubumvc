using System;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using HtmlTags;

namespace FubuMVC.Core.UI.Configuration
{
    public delegate HtmlTag TagBuilder(ElementRequest request);
    public delegate HtmlTag EachPartialTagBuilder(ElementRequest request, int index, int total);

    public delegate void TagModifier(ElementRequest request, HtmlTag tag);
    public delegate void EachPartialTagModifier(ElementRequest request, HtmlTag tag, int index, int total);

    public interface IElementBuilder
    {
        TagBuilder CreateInitial(AccessorDef accessorDef);
    }

    public interface IPartialElementBuilder
    {
        EachPartialTagBuilder CreateInitial(AccessorDef accessorDef);
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

    public abstract class EachOfPartialBuilder : IPartialElementBuilder
    {
        public EachPartialTagBuilder CreateInitial(AccessorDef accessorDef)
        {
            if (matches(accessorDef)) return Build;
            return null;
        }

        protected abstract bool matches(AccessorDef def);
        public abstract HtmlTag Build(ElementRequest request, int index, int total);
    }

    public interface IElementModifier
    {
        TagModifier CreateModifier(AccessorDef accessorDef);
    }

    public interface IPartialElementModifier
    {
        EachPartialTagModifier CreateModifier(AccessorDef accessorDef);
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

    //TODO: Work in support for other IElementNamingConventions such as this one
    public class DotNotationElementNamingConvention : IElementNamingConvention
    {
        public static Func<string, bool> IsCollectionIndexer = x => x.StartsWith("[") && x.EndsWith("]");

        public string GetName(Type modelType, Accessor accessor)
        {
            return accessor.PropertyNames
                .Aggregate((x, y) =>
                    {
                        var formatString = IsCollectionIndexer(y)
                                               ? "{0}{1}"
                                               : "{0}.{1}";
                        return formatString.ToFormat(x, y);
                    });
        }
    }
}