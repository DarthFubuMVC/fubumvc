using System;
using Spark;

namespace FubuMVC.Spark.Rendering
{
    public class ViewDefinition
    {
        public ViewDefinition(SparkViewDescriptor viewDescriptor, SparkViewDescriptor partialDescriptor, Type viewModel)
        {
            ViewDescriptor = viewDescriptor;
            PartialDescriptor = partialDescriptor;
            ViewModel = viewModel;
        }
        public SparkViewDescriptor ViewDescriptor { get; private set; }
        public SparkViewDescriptor PartialDescriptor { get; private set; }
        public Type ViewModel { get; private set; }
    }
}