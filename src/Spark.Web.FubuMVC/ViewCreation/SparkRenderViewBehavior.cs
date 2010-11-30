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
        private readonly IServiceLocator _serviceLocator;
        private readonly ISparkViewRenderer<IFubuView> _viewRenderder;
        private readonly SparkViewToken _viewToken;
        private readonly IOutputWriter _writer;

        public SparkRenderViewBehavior(
            ISparkViewRenderer<IFubuView> viewRenderder,
            IServiceLocator serviceLocator,
            IFubuRequest request,
            SparkViewToken viewToken,
            ActionCall actionCall,
            IOutputWriter writer)
            : base(PartialBehavior.Executes)
        {
            _viewRenderder = viewRenderder;
            _serviceLocator = serviceLocator;
            _request = request;
            _viewToken = viewToken;
            _actionCall = actionCall;
            _writer = writer;
        }

        protected override DoNext performInvoke()
        {
            lock (_actionCall)
            {
                string output = _viewRenderder.RenderSparkView(
                    _viewToken, _actionCall, view =>
                                                 {
                                                     var page = view as IFubuPage;
                                                     if (page != null)
                                                         page.ServiceLocator = _serviceLocator;

                                                     var viewWithModel = view as IFubuViewWithModel;
                                                     if (viewWithModel != null)
                                                         viewWithModel.SetModel(_request);
                                                 });

                string contentType = MimeType.Html.ToString();

                if (_viewToken.MatchedDescriptor != null && _viewToken.MatchedDescriptor.Language == LanguageType.Javascript)
                    contentType = MimeType.Javascript.ToString();

                _writer.Write(contentType, output);
            }
            return DoNext.Continue;
        }
    }
}