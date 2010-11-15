namespace FubuMVC.Core.View.WebForms
{
    public interface IPartialViewTypeBindingExpression
    {
        void Use<TPartialView>()
            where TPartialView : IFubuPage;
    }
}