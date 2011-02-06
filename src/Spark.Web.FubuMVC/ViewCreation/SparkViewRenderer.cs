using System;
using System.IO;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public interface ISparkViewRenderer<T>
    {
        string RenderSparkView(SparkViewToken viewToken, ActionCall actionCall, Action<T> configureView);
    }

    public class SparkViewRenderer<T> : ISparkViewRenderer<T> where T : class
    {
        private readonly ViewContextHolder _viewContextHolder;
        private readonly ISparkViewFactory _viewFactory;

        public SparkViewRenderer(ISparkViewFactory viewFactory, ViewContextHolder viewContextHolder)
        {
            _viewFactory = viewFactory;
            _viewContextHolder = viewContextHolder;
        }

        public string RenderSparkView(SparkViewToken viewToken, ActionCall actionCall, Action<T> configureView)
        {
            var actionNamespace = actionCall.HandlerType.Namespace;
            var actionName = viewToken.ActionName;
            var viewName = viewToken.Name;
            
            TextWriter writer = new StringWriter();

            if (viewToken.MatchedDescriptor != null && viewToken.MatchedDescriptor.Language == LanguageType.Javascript)
            {
                var entry = _viewFactory.Engine.CreateEntry(viewToken.MatchedDescriptor);
                return entry.SourceCode;
            }
            
            var viewResult = findSparkViewByConvention(actionNamespace, actionName, viewName);

            if (_viewContextHolder.OuterViewContext == null)
            {
                _viewContextHolder.OuterViewContext = new ViewContext(viewResult.ActionContext, viewResult.View);
            }

            var configurableView = viewResult.View as T;
            if (configurableView != null)
            {
                configureView(configurableView);
            }

            var sparkView = viewResult.View as SparkView;
            if (sparkView != null)
            {
                sparkView.Render(_viewContextHolder.OuterViewContext, writer);
            }

            return writer.ToString();
        }

        private ViewSearchResult findSparkViewByConvention(string actionNamespace, string actionName, string viewName)
        {
            var actionContext = new ActionContext(actionNamespace, actionName, d => d.Add("controller", actionName));
            
            var view = _viewContextHolder.OuterViewContext == null 
                ? _viewFactory.FindView(actionContext, viewName, null)
                : _viewFactory.FindPartialView(actionContext, viewName);

            return new ViewSearchResult(view.View, actionContext);
        }

        public class ViewSearchResult
        {
            public ViewSearchResult(ISparkView view, ActionContext actionContext)
            {
                View = view;
                ActionContext = actionContext;
            }

            public ISparkView View { get; private set; }
            public ActionContext ActionContext { get; private set; }
        }

        public class ViewContextHolder
        {
            public ViewContext OuterViewContext { get; set; }
        }
    }
}