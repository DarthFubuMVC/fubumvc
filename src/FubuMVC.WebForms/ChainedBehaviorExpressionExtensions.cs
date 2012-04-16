using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.WebForms
{
    public static class ChainedBehaviorExpressionExtensions
    {
        public static ExplicitRouteConfiguration.ChainedBehaviorExpression RenderWebForm<TWebform>(this ExplicitRouteConfiguration.ChainedBehaviorExpression expression)
        {
            // TODO -- blow up if the web form T is strongly typed to something that 
            // does not match the output type of the output type
            // Do it in validation rules


            var node = new WebFormView(typeof(TWebform));

            return expression.OutputTo(node);
        }
    }
}