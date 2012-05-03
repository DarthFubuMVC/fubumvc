using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Conditionals
{
    public class IsAjaxRequest : LambdaConditional<IRequestData>
    {
        public IsAjaxRequest(IRequestData context) : base(context, x => x.IsAjaxRequest())
        {
        }
    }
}