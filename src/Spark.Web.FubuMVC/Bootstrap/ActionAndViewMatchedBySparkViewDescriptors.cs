using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using Spark.Web.FubuMVC.ViewCreation;

namespace Spark.Web.FubuMVC.Bootstrap
{
    public class ActionAndViewMatchedBySparkViewDescriptors : IViewsForActionFilter
    {
        private readonly Func<Type, string> _getViewLocatorNameFromActionType;
        private readonly Func<ActionCall, string> _getViewNameFromActionCall;

        public ActionAndViewMatchedBySparkViewDescriptors(Func<Type, string> getViewLocatorNameFromActionType, Func<ActionCall, string> getViewNameFromActionCall)
        {
            _getViewLocatorNameFromActionType = getViewLocatorNameFromActionType;
            _getViewNameFromActionCall = getViewNameFromActionCall;
        }

        #region IViewsForActionFilter Members

        public IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views)
        {
            string viewName = _getViewNameFromActionCall(call);
            string viewLocatorName = _getViewLocatorNameFromActionType(call.HandlerType);
            IEnumerable<SparkViewToken> allViewTokens =
                views.Views.Where(view =>
                    view.GetType().CanBeCastTo<SparkViewToken>()).Cast<SparkViewToken>();

            SparkViewDescriptor matchedDescriptor = null;
            allViewTokens.FirstOrDefault(
                token =>
                {
                    matchedDescriptor = token.Descriptors
                        .Where(e => e.Templates
                                        .Any(template => template.Contains(viewLocatorName) && template.Contains(viewName)))
                        .SingleOrDefault();

                    return matchedDescriptor != null;
                });


            IEnumerable<IViewToken> viewsBoundToActions =
                matchedDescriptor != null
                    ? new IViewToken[] { new SparkViewToken(call, matchedDescriptor, viewLocatorName, viewName) }
                    : new IViewToken[0];

            return viewsBoundToActions;
        }

        #endregion
    }
}
