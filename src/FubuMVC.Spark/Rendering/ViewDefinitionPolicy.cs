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
        public bool Matches(ViewDescriptor descriptor)
        {
            return true;
        }

        public ViewDefinition Create(ViewDescriptor descriptor)
        {
            return descriptor.ToViewDefinition();
        }
    }
}