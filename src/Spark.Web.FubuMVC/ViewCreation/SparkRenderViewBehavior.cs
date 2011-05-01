using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Activation;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public class SparkRenderViewBehavior : BasicBehavior
    {
        private readonly ActionCall _actionCall;
        private readonly ISparkViewRenderer<IFubuPage> _viewRenderder;
        private readonly SparkViewToken _viewToken;
        private readonly IOutputWriter _writer;
        private readonly IPageActivator _activator;

        public SparkRenderViewBehavior(ISparkViewRenderer<IFubuPage> viewRenderder, SparkViewToken viewToken, ActionCall actionCall, IOutputWriter writer, IPageActivator activator)
            : base(PartialBehavior.Executes)
        {
            _viewRenderder = viewRenderder;
            _viewToken = viewToken;
            _actionCall = actionCall;
            _writer = writer;
            _activator = activator;
        }

        protected override DoNext performInvoke()
        {
            lock (_actionCall)
            {
                var output = _viewRenderder.RenderSparkView(_viewToken, _actionCall, view => _activator.Activate(view));

                var contentType = MimeType.Html.ToString();

                if (_viewToken.MatchedDescriptor != null &&
                    _viewToken.MatchedDescriptor.Language == LanguageType.Javascript)
                    contentType = MimeType.Javascript.ToString();

                _writer.Write(contentType, output);
            }
            return DoNext.Continue;
        }
    }
}