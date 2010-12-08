using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using Spark.Web.FubuMVC.Registration;
using Spark.Web.FubuMVC.ViewCreation;

namespace Spark.Web.FubuMVC.ViewLocation
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly ISparkViewFactory _viewFactory;
        private readonly ISparkPolicyResolver _policyResolver;

        public SparkViewFacility(ISparkViewFactory viewFactory, ISparkPolicyResolver policyResolver)
        {
            _viewFactory = viewFactory;
            _policyResolver = policyResolver;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            var viewTokens = new List<IViewToken>();
            graph
                .Actions()
                .Where(call => _policyResolver.HasMatchFor(call))
                .Each(call =>
                            {
                                var sparkBatchEntry = new SparkBatchEntry { ControllerType = call.HandlerType };
                                string viewLocatorName = _policyResolver.ResolveViewLocator(call);
                                IList<SparkViewDescriptor> descriptors = _viewFactory.CreateDescriptors(sparkBatchEntry, viewLocatorName);
                                viewTokens.Add(new SparkViewToken(descriptors));
                            });
            return viewTokens;
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            throw new NotSupportedException("This feature has not yet been implemented for Spark");
        }
    }
}