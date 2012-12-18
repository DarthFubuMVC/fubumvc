using FubuCore.Reflection;

namespace FubuMVC.Core.Registration
{
    public interface IAccessorRulesRegistration
    {
        void AddRules(AccessorRules rules);
    }
}