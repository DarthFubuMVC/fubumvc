using FubuCore.Reflection;
using FubuMVC.UI.Configuration;

namespace FubuMVC.UI.Security
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