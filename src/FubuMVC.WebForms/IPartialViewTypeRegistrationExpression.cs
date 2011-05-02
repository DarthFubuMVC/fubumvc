namespace FubuMVC.WebForms
{
    public interface IPartialViewTypeRegistrationExpression
    {
        IPartialViewTypeBindingExpression For<TPartialModel>();
    }
}