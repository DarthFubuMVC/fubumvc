using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Conditionals
{
    public class IsAjaxRequest : LambdaConditional<IRequestData>
    {
        public IsAjaxRequest()
            : base(x => x.IsAjaxRequest())
        {
        }
    }
}