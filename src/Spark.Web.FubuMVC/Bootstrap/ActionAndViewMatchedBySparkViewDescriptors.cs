using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using Spark.Web.FubuMVC.ViewCreation;

namespace Spark.Web.FubuMVC.Bootstrap
{
    public class ActionAndViewMatchedBySparkViewDescriptors : IViewsForActionFilter
    {
        private readonly Func<string, string> _actionNameFromActionCallConvention;

        public ActionAndViewMatchedBySparkViewDescriptors(Func<string, string> getActionNameFromCallConvention)
        {
            _actionNameFromActionCallConvention = getActionNameFromCallConvention;
        }

        #region IViewsForActionFilter Members

        public IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views)
        {
            string viewName = call.Method.Name;
            string actionName = _actionNameFromActionCallConvention(call.HandlerType.Name);

            SparkViewToken matchedView = views.Views.Cast<SparkViewToken>().ToList().Find(
                m =>
                    {
                        foreach (SparkViewDescriptor descriptor in m.Descriptors)
                            return (descriptor.Templates.Exists(
                                template => template.Contains(actionName) && template.Contains(viewName)));
                        return false;
                    });

            IEnumerable<IViewToken> viewsBoundToActions =
                matchedView != null
                    ? new IViewToken[] {new SparkViewToken(call, matchedView.Descriptors, actionName)}
                    : new IViewToken[0];

            return viewsBoundToActions;
        }

        #endregion
    }
}