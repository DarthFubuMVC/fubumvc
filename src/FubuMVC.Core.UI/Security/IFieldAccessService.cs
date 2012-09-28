using System.Reflection;
using FubuMVC.Core.UI.Elements;

namespace FubuMVC.Core.UI.Security
{
    public interface IFieldAccessService
    {
        AccessRight RightsFor(ElementRequest request);
        AccessRight RightsFor(object target, PropertyInfo property);
    }
}