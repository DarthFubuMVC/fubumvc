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
            IEnumerable<SparkViewToken> allViewTokens =
                views.Views.Cast<SparkViewToken>();

            SparkViewDescriptor matchedDescriptor = null;
            allViewTokens.FirstOrDefault(
                token =>
                {
                    matchedDescriptor = token.Descriptors
                        .Where(e => e.Templates
                                        .Exists(template => template.Contains(actionName) && template.Contains(viewName)))
                        .SingleOrDefault();
                    return matchedDescriptor != null;
                });

            IEnumerable<IViewToken> viewsBoundToActions =
                matchedDescriptor != null
                    ? new IViewToken[] { new SparkViewToken(call, matchedDescriptor, actionName) }
                    : new IViewToken[0];

            return viewsBoundToActions;
        }

        #endregion
    }
}
