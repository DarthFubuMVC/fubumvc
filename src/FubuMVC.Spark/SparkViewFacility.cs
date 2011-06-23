using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly ITemplateRegistry _templateRegistry;

        public SparkViewFacility(ITemplateRegistry templateRegistry)
        {
            _templateRegistry = templateRegistry;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            // clean up pending
            return _templateRegistry
                .AllTemplates()
                .Where(x => x.Descriptor is ViewDescriptor)
                .Select(x => x.Descriptor.As<ViewDescriptor>())
                .Where(x => x.HasViewModel())
                .Select(x => new SparkViewToken(x));
        }
    }
}