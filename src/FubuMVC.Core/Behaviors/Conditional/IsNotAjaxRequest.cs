using FubuCore.Binding;

namespace FubuMVC.Core.Behaviors.Conditional
{
    public class IsNotAjaxRequest : LambdaConditional<IRequestData>
    {
        public IsNotAjaxRequest(IRequestData context) : base(context, x => !x.IsAjaxRequest())
        {
        }
    }
}