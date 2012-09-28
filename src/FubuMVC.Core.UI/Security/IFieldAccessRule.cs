using FubuCore.Reflection;
using FubuHtml.Elements;

namespace FubuHtml.Security
{
    public enum FieldAccessCategory
    {
        LogicCondition,
        Authorization
    }

    public interface IFieldAccessRule
    {
        AccessRight RightsFor(ElementRequest request);
        bool Matches(Accessor accessor);
        FieldAccessCategory Category { get; }
    }
}