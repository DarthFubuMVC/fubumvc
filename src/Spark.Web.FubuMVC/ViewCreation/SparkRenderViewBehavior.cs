using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public class SparkRenderViewBehavior : BasicBehavior
    {
        private readonly ActionCall _actionCall;
        private readonly IFubuRequest _request;
        private readonly ISparkViewRenderer<IFubuView> _viewRenderder;
        private readonly SparkViewToken _viewToken;

        public SparkRenderViewBehavior(ISparkViewRenderer<IFubuView> viewRenderder, IFubuRequest request, SparkViewToken viewToken, ActionCall actionCall)
            : base(PartialBehavior.Executes)
        {
            _viewRenderder = viewRenderder;
            _request = request;
            _viewToken = viewToken;
            _actionCall = actionCall;
        }

        protected override DoNext performInvoke()
        {
            _viewRenderder.RenderSparkView(_viewToken, _actionCall,
                                           view =>
                                               {
                                                   var viewWithModel = view as IFubuViewWithModel;
                                                   if (viewWithModel != null)
                                                       viewWithModel.SetModel(_request);
                                               });
            return DoNext.Continue;
        }
    }
}