using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View;
using FubuMVC.Core.View.Model;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly ITemplateRegistry<ITemplate> _templateRegistry;

        public SparkViewFacility(ITemplateRegistry<ITemplate> templateRegistry)
        {
            _templateRegistry = templateRegistry;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            // TODO: Make it not filter against view model as the 
            // default view attacher conventions will do this.
            // Opens up for returning view with no model in edge cases
            // and add custom view attacher convention.

            return _templateRegistry
                .AllTemplates()
                .Where(x => x.Descriptor is SparkDescriptor)
                .Select(x => x.Descriptor.As<SparkDescriptor>())
                .Where(x => x.HasViewModel())
                .Select(x => new SparkViewToken(x));
        }
    }
}