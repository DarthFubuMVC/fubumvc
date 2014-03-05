using FubuCore;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    [MarkedForTermination("Silly thing doesn't really deserve to live.")]
    public class ViewDefinition
    {
        public ViewDefinition(SparkViewDescriptor viewDescriptor, SparkViewDescriptor partialDescriptor)
        {
            ViewDescriptor = viewDescriptor;
            PartialDescriptor = partialDescriptor;
        }
        public SparkViewDescriptor ViewDescriptor { get; private set; }
        public SparkViewDescriptor PartialDescriptor { get; private set; }
    }
}