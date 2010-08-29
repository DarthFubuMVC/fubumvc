using System;
using System.Web;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public interface ISparkViewRenderer<T>
    {
        void RenderSparkView(SparkViewToken viewToken, ActionCall actionCall, Action<T> configureView);
    }

    public class SparkViewRenderer<T> : ISparkViewRenderer<T> where T : class
    {
        private readonly HttpContextBase _httpContext;
        private readonly SparkViewFactory _viewFactory;
        private readonly IOutputWriter _writer;

        public SparkViewRenderer(SparkViewFactory viewFactory, HttpContextBase httpContext, IOutputWriter writer)
        {
            _viewFactory = viewFactory;
            _httpContext = httpContext;
            _writer = writer;
        }

        #region ISparkViewRenderer<T> Members

        public void RenderSparkView(SparkViewToken viewToken, ActionCall actionCall, Action<T> configureView)
        {
            string actionNamespace = actionCall.HandlerType.Namespace;
            string actionName = viewToken.ActionName;
            string viewName = viewToken.Name;

            if (viewToken.MatchedDescriptor != null && viewToken.MatchedDescriptor.Language == LanguageType.Javascript)
            {
                var entry = _viewFactory.Engine.CreateEntry(viewToken.MatchedDescriptor);
                _writer.Write("text/javascript", entry.SourceCode);
                return;
            }

            ISparkView sparkView = _viewFactory.FindView(_httpContext, actionNamespace, actionName, viewName, null);

            var configurableView = sparkView as T;
            if (configurableView != null)
                configureView(configurableView);

            sparkView.RenderView(_httpContext.Response.Output);
        }

        #endregion
    }
}