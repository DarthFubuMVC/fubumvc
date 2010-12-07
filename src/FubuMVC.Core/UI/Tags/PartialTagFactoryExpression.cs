using System;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;

namespace FubuMVC.Core.UI.Tags
{
    public class PartialTagFactoryExpression
    {
        private readonly PartialTagFactory _factory;

        public PartialTagFactoryExpression(PartialTagFactory factory)
        {
            _factory = factory;
        }

        public PartialTagActionExpression Always { get { return If(def => true); } }

        public void Builder<T>() where T : IPartialElementBuilder, new()
        {
            Builder(new T());
        }

        public void Builder(IPartialElementBuilder builder)
        {
            _factory.AddBuilder(builder);
        }

        public void Modifier<T>() where T : IPartialElementModifier, new()
        {
            Modifier(new T());
        }

        public void Modifier(IPartialElementModifier modifier)
        {
            _factory.AddModifier(modifier);
        }

        public PartialTagActionExpression If(Func<AccessorDef, bool> matches)
        {
            return new PartialTagActionExpression(_factory, matches);
        }

        public PartialTagActionExpression IfPropertyTypeIs(Func<Type, bool> matches)
        {
            return If(def => matches(def.Accessor.PropertyType));
        }

        public PartialTagActionExpression IfPropertyIs<T>()
        {
            return If(def => def.Accessor.PropertyType == typeof(T));
        }

        public void AddClassForAttribute<T>(string className) where T : Attribute
        {
            If(def => def.Accessor.HasAttribute<T>()).AddClass(className);
        }

        public void ModifyForAttribute<T>(Action<HtmlTag, T> modification) where T : Attribute
        {
            EachPartialTagModifier modifier = (request, tag, index, count) =>
                                   request.Accessor.ForAttribute<T>(att => modification(tag, att));

            If(def => true).Modify(modifier);
        }

        public void ModifyForAttribute<T>(Action<HtmlTag> modification) where T : Attribute
        {
            ModifyForAttribute<T>((tag, att) => modification(tag));
        }
    }
}