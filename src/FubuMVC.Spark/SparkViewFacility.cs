using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly ITemplateComposer _composer;
        public SparkViewFacility(ITemplateComposer composer)
        {
            _composer = composer;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            return _composer.Compose(types)
                .AllTemplates()
                .Where(x => x.Descriptor is ViewDescriptor)
                .Select(x => x.Descriptor.As<ViewDescriptor>())
                .Where(x => x.HasViewModel())
                .Select(x => new SparkViewToken(x));
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            return null;
        }
    }
}