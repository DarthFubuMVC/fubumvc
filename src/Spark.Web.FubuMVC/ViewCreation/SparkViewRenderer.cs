﻿using System;
using System.IO;
using System.Web;
using System.Web.Routing;
using FubuMVC.Core.Registration.Nodes;

namespace Spark.Web.FubuMVC.ViewCreation
{
    public interface ISparkViewRenderer<T>
    {
        string RenderSparkView(SparkViewToken viewToken, ActionCall actionCall, Action<T> configureView);
    }

    public class SparkViewRenderer<T> : ISparkViewRenderer<T> where T : class
    {
        private static ViewContext _outerViewContext;
        private readonly HttpContextBase _httpContext;
        private readonly SparkViewFactory _viewFactory;

        public SparkViewRenderer(SparkViewFactory viewFactory, HttpContextBase httpContext)
        {
            _viewFactory = viewFactory;
            _httpContext = httpContext;
        }

        #region ISparkViewRenderer<T> Members

        public string RenderSparkView(SparkViewToken viewToken, ActionCall actionCall, Action<T> configureView)
        {
            string actionNamespace = actionCall.HandlerType.Namespace;
            string actionName = viewToken.ActionName;
            string viewName = viewToken.Name;
            TextWriter writer = new StringWriter();

            if (viewToken.MatchedDescriptor != null && viewToken.MatchedDescriptor.Language == LanguageType.Javascript)
            {
                ISparkViewEntry entry = _viewFactory.Engine.CreateEntry(viewToken.MatchedDescriptor);
                return entry.SourceCode;
            }

            ActionContext actionContext;
            ISparkView view = FindSparkViewByConvention(actionNamespace, actionName, viewName, out actionContext);
            if (_outerViewContext == null)
                _outerViewContext = new ViewContext(actionContext, view);

            var configurableView = view as T;
            if (configurableView != null)
                configureView(configurableView);

            var sparkView = view as SparkView;
            if (sparkView != null)
                sparkView.Render(_outerViewContext, writer);

            if(ReferenceEquals(sparkView, _outerViewContext.View))
                _outerViewContext = null;
            return writer.ToString();
        }

        #endregion

        private ISparkView FindSparkViewByConvention(string actionNamespace, string actionName, string viewName, out ActionContext actionContext)
        {
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