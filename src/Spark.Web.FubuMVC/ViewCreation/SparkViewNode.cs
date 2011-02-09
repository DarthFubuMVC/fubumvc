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
            get
            {
            	if(_viewToken.MatchedDescriptor == null)
            	{
					return string.Format("Spark View {0}", _viewToken.Name);
            	}

				return string.Format("Spark View {0} as {1}", _viewToken.Name, _viewToken.MatchedDescriptor.Language);
            }
        }

        protected override void configureObject(ObjectDef def)
        {
            def.Child(_viewToken);
            def.Child(_actionCall);
        }
    }
}