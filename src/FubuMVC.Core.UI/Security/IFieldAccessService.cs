using System.Reflection;
using FubuHtml.Elements;

namespace FubuHtml.Security
{
    public interface IFieldAccessService
    {
        AccessRight RightsFor(ElementRequest request);
        AccessRight RightsFor(object target, PropertyInfo property);
    }
}