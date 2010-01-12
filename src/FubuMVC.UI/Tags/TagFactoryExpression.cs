using System;
using FubuMVC.Core.Util;
using FubuMVC.UI.Configuration;
using HtmlTags;

namespace FubuMVC.UI.Tags
{
    public class TagFactoryExpression
    {
        private readonly TagFactory _factory;

        public TagFactoryExpression(TagFactory factory)
        {
            _factory = factory;
        }

        public TagActionExpression Always { get { return If(def => true); } }

        public void Builder<T>() where T : IElementBuilder, new()
        {
            Builder(new T());
        }

        public void Builder(IElementBuilder builder)
        {
            _factory.AddBuilder(builder);
        }

        public void Modifier<T>() where T : IElementModifier, new()
        {
            Modifier(new T());
        }

        public void Modifier(IElementModifier modifier)
        {
            _factory.AddModifier(modifier);
        }

        public TagActionExpression If(Func<AccessorDef, bool> matches)
        {
            return new TagActionExpression(_factory, matches);
        }


        public TagActionExpression IfPropertyIs<T>()
        {
            return If(def => def.Accessor.PropertyType == typeof (T));
        }

        public void AddClassForAttribute<T>(string className) where T : Attribute
        {
            If(def => def.Accessor.HasAttribute<T>()).AddClass(className);
        }

        public void ModifyForAttribute<T>(Action<HtmlTag, T> modification) where T : Attribute
        {
            TagModifier modifier = (request, tag) =>
                                   request.Accessor.ForAttribute<T>(att => modification(tag, att));

            If(def => true).Modify(modifier);
        }
    }
}