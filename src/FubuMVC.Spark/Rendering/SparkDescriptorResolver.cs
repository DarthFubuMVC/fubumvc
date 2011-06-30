using Spark;

namespace FubuMVC.Spark.Rendering
{
    public class SparkDescriptorResolver : ISparkDescriptorResolver
    {
        public SparkViewDescriptor ResolveDescriptor(ViewDefinition viewDefinition)
        {
            return viewDefinition.ViewDescriptor;
        }

        public SparkViewDescriptor ResolvePartialDescriptor(ViewDefinition viewDefinition)
        {
            return viewDefinition.PartialDescriptor;
        }
    }
    public interface ISparkDescriptorResolver
    {
        SparkViewDescriptor ResolveDescriptor(ViewDefinition viewDefinition);
        SparkViewDescriptor ResolvePartialDescriptor(ViewDefinition viewDefinition);
    }
}