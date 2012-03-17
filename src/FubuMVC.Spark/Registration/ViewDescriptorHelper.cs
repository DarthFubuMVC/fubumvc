using FubuMVC.Spark.Rendering;
using FubuMVC.Spark.SparkModel;
using Spark;

namespace FubuMVC.Spark.Registration
{
    public static class ViewDescriptorHelper
    {
        public static ViewDefinition ToViewDefinition(this SparkDescriptor viewDescriptor)
        {
            var normal = viewDescriptor.ToSparkViewDescriptor();
            var partial = viewDescriptor.ToPartialSparkViewDescriptor();
            return new ViewDefinition(normal, partial);
        }

        public static SparkViewDescriptor ToSparkViewDescriptor(this SparkDescriptor viewDescriptor)
        {
            return createSparkDescriptor(true, viewDescriptor);
        }

        public static SparkViewDescriptor ToPartialSparkViewDescriptor(this SparkDescriptor viewDescriptor)
        {
            return createSparkDescriptor(false, viewDescriptor);
        }


        private static SparkViewDescriptor createSparkDescriptor(bool useMaster, SparkDescriptor viewDescriptor)
        {
            var sparkDescriptor = new SparkViewDescriptor().AddTemplate(viewDescriptor.ViewPath);
            if (useMaster && viewDescriptor.Master != null)
            {
                appendMasterPage(sparkDescriptor, viewDescriptor.Master);
            }

            return sparkDescriptor;
        }

        private static void appendMasterPage(SparkViewDescriptor descriptor, ITemplate template)
        {
            if (template == null)
            {
                return;
            }
            descriptor.AddTemplate(template.ViewPath);
            var viewDescriptor = template.Descriptor as SparkDescriptor;
            if (viewDescriptor != null)
            {
                appendMasterPage(descriptor, viewDescriptor.Master);
            }
        }
    }
}