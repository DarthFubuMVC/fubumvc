using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    [MarkedForTermination("Duplication I'd like to see go away")]
    public interface ISparkTemplateRegistry : ITemplateRegistry<ISparkTemplate>
    {
        IEnumerable<ISparkTemplate> BindingsForView(string viewPath);
        IEnumerable<SparkDescriptor> ViewDescriptors();
    }

    public class SparkTemplateRegistry : TemplateRegistry<ISparkTemplate>, ISparkTemplateRegistry
    {
        public SparkTemplateRegistry() {}
        public SparkTemplateRegistry(IEnumerable<ISparkTemplate> templates) : base(templates) {}

        public IEnumerable<ISparkTemplate> BindingsForView(string viewPath)
        {
            return descriptors(t => t.ViewPath == viewPath)
                .SelectMany(t => t.Bindings)
                .ToList();
        }

        public IEnumerable<SparkDescriptor> ViewDescriptors()
        {
            return descriptors(t => t.IsSparkView()).ToList();
        }

        private IEnumerable<SparkDescriptor> descriptors(Func<ISparkTemplate, bool> selector)
        {
            return this.Where(t => t.Descriptor is SparkDescriptor && selector(t))
                .Select(t => t.Descriptor.As<SparkDescriptor>());
        }
    }
}