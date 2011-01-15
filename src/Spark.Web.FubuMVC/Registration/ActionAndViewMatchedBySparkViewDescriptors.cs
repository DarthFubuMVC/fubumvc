using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using Spark.Web.FubuMVC.Extensions;
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
            if(call.OutputType() == typeof(FubuContinuation) || !_policyResolver.HasMatchFor(call))
            {
                return new IViewToken[0];
            }

            var viewName = _policyResolver.ResolveViewName(call);
            var viewLocatorName = _policyResolver.ResolveViewLocator(call);
        	var allViewTokens = views
									.Views
									.Where(view => view.GetType().CanBeCastTo<SparkViewToken>())
									.Cast<SparkViewToken>();

        	SparkViewDescriptor matchedDescriptor = null;
        	foreach(var token in allViewTokens)
        	{
        		var view = viewName.RemoveSuffix(".spark");
				var templatePath = !string.IsNullOrEmpty(viewLocatorName) ? "{0}\\{1}".ToFormat(viewLocatorName, view) : view;
        		var descriptor = token
        							.Descriptors
        							.FirstOrDefault(d => d.Templates.Any(template => template.RemoveSuffix(".spark").Equals(templatePath)));

				if(descriptor != null)
				{
					matchedDescriptor = descriptor;
					break;
				}
        	}

            return matchedDescriptor != null
                    ? new IViewToken[] { new SparkViewToken(call, matchedDescriptor, viewLocatorName, viewName) }
                    : new IViewToken[0];
        }
    }
}
