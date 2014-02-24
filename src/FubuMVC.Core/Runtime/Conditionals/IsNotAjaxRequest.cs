using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Conditionals
{
    public class IsNotAjaxRequest : LambdaConditional<IRequestData>
    {
        public IsNotAjaxRequest() : base(x => !x.IsAjaxRequest())
        {
        }
    }
}