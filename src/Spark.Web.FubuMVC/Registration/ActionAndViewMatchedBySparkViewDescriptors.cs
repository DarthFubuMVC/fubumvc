using System.Collections.Generic;
using System.IO;
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
    	private readonly ISparkDescriptorVisitorRegistry _visitorRegistry;

        public ActionAndViewMatchedBySparkViewDescriptors(ISparkPolicyResolver policyResolver, ISparkDescriptorVisitorRegistry visitorRegistry)
        {
        	_policyResolver = policyResolver;
        	_visitorRegistry = visitorRegistry;
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
				var templatePath = !string.IsNullOrEmpty(viewLocatorName) ? "{0}{1}{2}".ToFormat(viewLocatorName, Path.DirectorySeparatorChar, view) : view;
        		var descriptor = token
        							.Descriptors
        							.FirstOrDefault(d => d.Templates.Any(template => template.RemoveSuffix(".spark").ToLower().Equals(templatePath.ToLower())));

				if(descriptor != null)
				{
					matchedDescriptor = descriptor;
					break;
				}
        	}

			if(matchedDescriptor == null)
			{
				return new IViewToken[0];
			}

    		_visitorRegistry
    			.VisitorsFor(call)
    			.Each(visitor => visitor.Visit(matchedDescriptor, call));

    		return new IViewToken[] {new SparkViewToken(call, matchedDescriptor, viewLocatorName, viewName)};
        }
    }
}
