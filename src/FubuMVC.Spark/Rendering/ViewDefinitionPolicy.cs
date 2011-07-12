using FubuCore.Util;
using FubuMVC.Spark.Registration;
using FubuMVC.Spark.SparkModel;

namespace FubuMVC.Spark.Rendering
{
    public interface IViewDefinitionPolicy
    {
        bool Matches(ViewDescriptor descriptor);
        ViewDefinition Create(ViewDescriptor descriptor);
    }

    public class DefaultViewDefinitionPolicy : IViewDefinitionPolicy
    {
        private readonly Cache<ViewDescriptor, ViewDefinition> _cache;

        public DefaultViewDefinitionPolicy()
        {
            _cache = new Cache<ViewDescriptor, ViewDefinition>(x => x.ToViewDefinition());
        }

        public bool Matches(ViewDescriptor descriptor)
        {
            return true;
        }

        public virtual ViewDefinition Create(ViewDescriptor descriptor)
        {
            return _cache[descriptor];
        }
    }
}