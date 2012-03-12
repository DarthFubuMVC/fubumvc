using FubuCore.Util;
using FubuMVC.Spark.Registration;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewDefinitionPolicy
    {
        bool Matches(SparkDescriptor descriptor);
        ViewDefinition Create(SparkDescriptor descriptor);
    }

    public class DefaultViewDefinitionPolicy : IViewDefinitionPolicy
    {
        private readonly Cache<SparkDescriptor, ViewDefinition> _cache;

        public DefaultViewDefinitionPolicy()
        {
            _cache = new Cache<SparkDescriptor, ViewDefinition>(x => x.ToViewDefinition());
        }

        public bool Matches(SparkDescriptor descriptor)
        {
            return true;
        }

        public virtual ViewDefinition Create(SparkDescriptor descriptor)
        {
            return _cache[descriptor];
        }
    }
}