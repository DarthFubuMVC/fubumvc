using System.Collections.Generic;
using System.Linq;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark.Rendering
{
    public class ViewDefinitionResolver : IViewDefinitionResolver
    {
        private readonly IEnumerable<IViewDefinitionPolicy> _policies;
        private readonly DefaultViewDefinitionPolicy _defaultPolicy;

        public ViewDefinitionResolver(IEnumerable<IViewDefinitionPolicy> policies, DefaultViewDefinitionPolicy defaultPolicy)
        {
            _policies = policies;
            _defaultPolicy = defaultPolicy;
        }

        public ViewDefinition Resolve(SparkDescriptor descriptor)
        {
            var policy = _policies.FirstOrDefault(x => x.Matches(descriptor)) ?? _defaultPolicy;
            return policy.Create(descriptor);
        }
    }
    public interface IViewDefinitionResolver
    {
        ViewDefinition Resolve(SparkDescriptor descriptor);
    }

}