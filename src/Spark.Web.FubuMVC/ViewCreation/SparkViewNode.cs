using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public class SparkViewNode : OutputNode
    {
        protected readonly ActionCall _actionCall;
        protected readonly SparkViewToken _viewToken;

        public SparkViewNode(SparkViewToken viewToken, ActionCall actionCall)
            : base(typeof (SparkRenderViewBehavior))
        {
            _viewToken = viewToken;
            _actionCall = actionCall;
        }

        public override string Description
        {
            get { return string.Format("Spark View {0}", _viewToken.Name); }
        }

        protected override void configureObject(ObjectDef def)
        {
            def.Child(_viewToken);
            def.Child(_actionCall);
        }
    }
}