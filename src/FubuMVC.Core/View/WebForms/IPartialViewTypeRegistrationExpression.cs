namespace FubuMVC.Core.View.WebForms
{
    public interface IPartialViewTypeRegistrationExpression
    {
        IPartialViewTypeBindingExpression For<TPartialModel>();
    }
}