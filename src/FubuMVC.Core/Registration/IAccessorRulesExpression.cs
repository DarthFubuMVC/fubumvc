namespace FubuMVC.Core.Registration
{
    public interface IAccessorRulesExpression
    {
        IAccessorRulesExpression Add(object rule);
        IAccessorRulesExpression Add<T>() where T : new();
    }
}