using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Conditionals
{
    public class IsNotAjaxRequest : LambdaConditional<IRequestData>
    {
        public IsNotAjaxRequest(IRequestData context) : base(context, x => !x.IsAjaxRequest())
        {
        }
    }
}