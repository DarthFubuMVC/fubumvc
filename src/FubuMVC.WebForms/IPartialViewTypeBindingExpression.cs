using FubuMVC.Core.View;

namespace FubuMVC.WebForms
{
    public interface IPartialViewTypeBindingExpression
    {
        void Use<TPartialView>()
            where TPartialView : IFubuPage;
    }
}