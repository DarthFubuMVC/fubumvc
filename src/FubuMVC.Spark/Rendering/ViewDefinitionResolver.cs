using System.Collections.Generic;
using System.Linq;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark.Rendering
{
    public class ViewDefinitionResolver : IViewDefinitionResolver
    {
        private readonly IList<IViewDefinitionPolicy> _policies = new List<IViewDefinitionPolicy>();

        public ViewDefinitionResolver(IEnumerable<IViewDefinitionPolicy> policies, DefaultViewDefinitionPolicy defaultPolicy)
        {
            _policies.AddRange(policies);
            _policies.Add(defaultPolicy);
        }

        public ViewDefinition Resolve(ViewDescriptor descriptor)
        {
            var policy = _policies.First(x => x.Matches(descriptor));
            return policy.Create(descriptor);
        }
    }
    public interface IViewDefinitionResolver
    {
        ViewDefinition Resolve(ViewDescriptor descriptor);
    }

}