using FubuCore.Binding;

namespace FubuMVC.Core.Behaviors.Conditional
{
    public class IsAjaxRequest : LambdaConditional<IRequestData>
    {
        public IsAjaxRequest(IRequestData context) : base(context, x => x.IsAjaxRequest())
        {
        }
    }
}