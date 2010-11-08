using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using Spark.Web.FubuMVC.ViewCreation;

namespace Spark.Web.FubuMVC.Registration
{
    public class ActionAndViewMatchedBySparkViewDescriptors : IViewsForActionFilter
    {
        private readonly ISparkPolicyResolver _policyResolver;

        public ActionAndViewMatchedBySparkViewDescriptors(ISparkPolicyResolver policyResolver)
        {
            _policyResolver = policyResolver;
        }

        public IEnumerable<IViewToken> Apply(ActionCall call, ViewBag views)
        {
            if(!_policyResolver.HasMatchFor(call))
            {
                return new IViewToken[0];
            }

            string viewName = _policyResolver.ResolveViewName(call);
            string viewLocatorName = _policyResolver.ResolveViewLocator(call);
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
    }
}
