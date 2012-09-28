using System;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuCore;

namespace FubuMVC.Core.UI.Elements.Builders
{
    public class AddClassForAttributeModifier<T> : IElementModifier, DescribesItself where T : Attribute
    {
        private readonly string _className;

        public AddClassForAttributeModifier(string className)
        {
            _className = className;
        }

        public bool Matches(ElementRequest token)
        {
            return token.Accessor.HasAttribute<T>();
        }

        public void Modify(ElementRequest request)
        {
            request.CurrentTag.AddClass(_className);
        }

        public void Describe(Description description)
        {
            description.Title = "Adds class '{0}' if the accessor is decorated with attribute [{1}]"
                .ToFormat(_className, typeof (T).Name);
        }
    }
}