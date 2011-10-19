using System;
using System.Linq;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Configuration;
using HtmlTags;

namespace FubuMVC.Core.UI.Tags
{
    public class FormTagFactoryExpression
    {
        private readonly FormTagFactory _factory;

        public FormTagFactoryExpression(FormTagFactory factory)
        {
            _factory = factory;
        }

        public FormTagActionExpression Always { get { return If(def => true); } }

        public void Modifier<T>() where T : IFormElementModifier, new()
        {
            Modifier(new T());
        }

        public void Modifier(IFormElementModifier modifier)
        {
            _factory.AddModifier(modifier);
        }

        public FormTagActionExpression If(Func<FormDef, bool> matches)
        {
            return new FormTagActionExpression(_factory, matches);
        }

        public FormTagActionExpression IfInBound()
        {
            return If(x => x.IsInBound == true);
        }

        public FormTagActionExpression IfOutBound()
        {
            return If(x => x.IsInBound == false);
        }

        public void AddClassForAttribute<T>(string className) where T : Attribute
        {         
            If(def => def.ModelType.HasAttribute<T>()).AddClass(className);
        }

        public void ModifyForAttribute<T>(Action<FormTag, T> modification) where T : Attribute
        {
            FormTagModifier modifier = (request, tag) =>
                                   request.ModelType.ForAttribute<T>(att => modification(tag, att));

            If(def => true).Modify(modifier);
        }

        public void ModifyForAttribute<T>(Action<FormTag> modification) where T : Attribute
        {
            ModifyForAttribute<T>((tag, att) => modification(tag));
        }
    }
}