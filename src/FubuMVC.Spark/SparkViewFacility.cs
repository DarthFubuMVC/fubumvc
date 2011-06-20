using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.View;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark
{
    public class SparkViewFacility : IViewFacility
    {
        private readonly ITemplateRegistry _templateRegistry;
        private readonly ITemplateFilterComposer _filterComposer;

        public SparkViewFacility(ITemplateRegistry templateRegistry, ITemplateFilterComposer filterComposer)
        {
            _templateRegistry = templateRegistry;
            _filterComposer = filterComposer;
        }

        public IEnumerable<IViewToken> FindViews(TypePool types, BehaviorGraph graph)
        {
            // clean up pending
            return _templateRegistry
                .AllTemplates()
                .Where(x => _filterComposer.Matches(x) && x.Descriptor is ViewDescriptor)
                .Select(x => x.Descriptor.As<ViewDescriptor>())
                .Where(x => x.HasViewModel())
                .Select(x => new SparkViewToken(x));
        }

        public BehaviorNode CreateViewNode(Type type)
        {
            return null;
        }
    }

    public interface ITemplateFilterComposer
    {
        bool Matches(ITemplate template);
    }

    public class TemplateFilterComposer : ITemplateFilterComposer
    {
        private readonly CompositeFilter<ITemplate> _filters = new CompositeFilter<ITemplate>();

        public TemplateFilterComposer(IEnumerable<ITemplateFilter> filters)
        {
            _filters.Includes += (t => true);
            filters
                .Each(f => _filters.Excludes.Add(t => f.Exclude(t)));
        }

        public bool Matches(ITemplate template)
        {
            return _filters.Matches(template);
        }
    }

    public interface ITemplateFilter
    {
        bool Exclude(ITemplate template);
    }

    public class LambdaTemplateFilter : ITemplateFilter
    {
        private readonly Func<ITemplate, bool> _predicate;

        public LambdaTemplateFilter(Func<ITemplate, bool> predicate)
        {
            _predicate = predicate;
        }

        public bool Exclude(ITemplate template)
        {
            return _predicate(template);
        }
    }
}