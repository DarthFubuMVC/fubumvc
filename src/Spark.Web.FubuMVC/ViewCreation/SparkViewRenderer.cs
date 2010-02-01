using System;
using System.Web;
using FubuMVC.Core.Registration.Nodes;

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

        public SparkViewRenderer(SparkViewFactory viewFactory, HttpContextBase httpContext)
        {
            _viewFactory = viewFactory;
            _httpContext = httpContext;
        }

        #region ISparkViewRenderer<T> Members

        public void RenderSparkView(SparkViewToken viewToken, ActionCall actionCall, Action<T> configureView)
        {
            string actionNamespace = actionCall.HandlerType.Namespace;
            string actionName = viewToken.ActionName;
            string viewName = viewToken.Name;

            ISparkView sparkView = _viewFactory.FindView(_httpContext, actionNamespace, actionName, viewName, null);

            var configurableView = sparkView as T;
            if (configurableView != null)
                configureView(configurableView);

            sparkView.RenderView(_httpContext.Response.Output);
        }

        #endregion
    }
}