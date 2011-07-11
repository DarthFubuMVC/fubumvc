using System.Collections.Generic;
using System.Linq;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewEntrySource
    {
        ISparkViewEntry GetViewEntry();
        ISparkViewEntry GetPartialViewEntry();
    }

    public class ViewEntrySource : IViewEntrySource
    {
        private readonly IList<IViewDefinitionPolicy> _policies = new List<IViewDefinitionPolicy>();
        private readonly IViewEntryProviderCache _provider;
        private readonly ViewDescriptor _descriptor;

        public ViewEntrySource(ViewDescriptor descriptor, 
            IViewEntryProviderCache provider, 
            IEnumerable<IViewDefinitionPolicy> policies,
            DefaultViewDefinitionPolicy defaultPolicy)
        {
            _descriptor = descriptor;
            _provider = provider;
            _policies.AddRange(policies);
            _policies.Add(defaultPolicy);
        }

        public ISparkViewEntry GetViewEntry()
        {
            return getViewEntry(false);
        }

        public ISparkViewEntry GetPartialViewEntry()
        {
            return getViewEntry(true);
        }

        private ISparkViewEntry getViewEntry(bool partial)
        {
            var policy = _policies.First(x => x.Matches(_descriptor));
            var definition = policy.Create(_descriptor);
            var sparkDescriptor = partial ? definition.PartialDescriptor : definition.ViewDescriptor;
            return _provider.GetViewEntry(sparkDescriptor);
        }

    }
}