using FubuCore.Descriptions;
using FubuCore;

namespace FubuMVC.Core.UI.Elements.Builders
{
    public class AddClassModifier : IElementModifier, DescribesItself
    {
        private readonly string _className;

        public AddClassModifier(string className)
        {
            _className = className;
        }

        public void Describe(Description description)
        {
            description.Title = "Add class '{0}' to the element".ToFormat(_className);
        }

        public bool Matches(ElementRequest token)
        {
            return true;
        }

        public void Modify(ElementRequest request)
        {
            request.CurrentTag.AddClass(_className);
        }
    }
}