using System;
using System.Web;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using System.IO;
using System.Web.Routing;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public interface ISparkViewRenderer<T>
    {
        void RenderSparkView(SparkViewToken viewToken, ActionCall actionCall, Action<T> configureView);
    }

    public class SparkViewRenderer<T> : ISparkViewRenderer<T> where T : class
    {
        private static ViewContext _outerViewContext;
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
            TextWriter writer = _httpContext.Response.Output;

            if (viewToken.MatchedDescriptor != null && viewToken.MatchedDescriptor.Language == LanguageType.Javascript)
            {
                var entry = _viewFactory.Engine.CreateEntry(viewToken.MatchedDescriptor);
                _writer.Write("text/javascript", entry.SourceCode);
                return;
            }

            ActionContext actionContext;
            ISparkView view = FindSparkViewByConvention(actionNamespace, actionName, viewName, out actionContext);
            if (_outerViewContext == null)
                _outerViewContext = new ViewContext(actionContext, view, writer);

            var configurableView = view as T;
            if (configurableView != null)
                configureView(configurableView);

            var sparkView = view as SparkView;
            if (sparkView != null) 
                sparkView.Render(_outerViewContext, writer);

            _outerViewContext = null;
        }

        #endregion
        private ISparkView FindSparkViewByConvention(string actionNamespace, string actionName, string viewName, out ActionContext actionContext)
        {
            //TODO: Rob G - This is where we need to feed in convention to find views in same folder as controller

            var routeData = new RouteData();
            routeData.Values.Add("controller", actionName);
            actionContext = new ActionContext(_httpContext, routeData, actionNamespace, actionName);
            ViewEngineResult findResult = _outerViewContext == null 
                ? _viewFactory.FindView(actionContext, viewName, null)
                : _viewFactory.FindPartialView(actionContext, viewName);

            return findResult.View;
        }
    }
}