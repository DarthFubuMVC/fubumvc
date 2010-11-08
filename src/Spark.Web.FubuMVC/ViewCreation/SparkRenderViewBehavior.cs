using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using Microsoft.Practices.ServiceLocation;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public class SparkRenderViewBehavior : BasicBehavior
    {
        private readonly ActionCall _actionCall;
        private readonly IFubuRequest _request;
        private readonly ISparkViewRenderer<IFubuView> _viewRenderder;
        private readonly IServiceLocator _serviceLocator;
        private readonly SparkViewToken _viewToken;

        public SparkRenderViewBehavior(ISparkViewRenderer<IFubuView> viewRenderder, IServiceLocator serviceLocator, IFubuRequest request, SparkViewToken viewToken, ActionCall actionCall)
            : base(PartialBehavior.Executes)
        {
            _viewRenderder = viewRenderder;
            _serviceLocator = serviceLocator;
            _request = request;
            _viewToken = viewToken;
            _actionCall = actionCall;
        }

        protected override DoNext performInvoke()
        {
            _viewRenderder.RenderSparkView(_viewToken, _actionCall,
                                           view =>
                                               {
                                                   var page = view as IFubuPage;
                                                   if(page != null)
                                                   {
                                                       page.ServiceLocator = _serviceLocator;
                                                   }

                                                   var viewWithModel = view as IFubuViewWithModel;
                                                   if (viewWithModel != null)
                                                       viewWithModel.SetModel(_request);
                                               });
            return DoNext.Continue;
        }
    }
}