using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkTemplateRegistry : ITemplateRegistry<ITemplate>
    {
        IEnumerable<ITemplate> BindingsForView(string viewPath);
        IEnumerable<SparkDescriptor> ViewDescriptors();
    }

    public class SparkTemplateRegistry : TemplateRegistry<ITemplate>, ISparkTemplateRegistry
    {
        public SparkTemplateRegistry() {}
        public SparkTemplateRegistry(IEnumerable<ITemplate> templates) : base(templates) {}

        public IEnumerable<ITemplate> BindingsForView(string viewPath)
        {
            return descriptors(t => t.ViewPath == viewPath)
                .SelectMany(t => t.Bindings)
                .ToList();
        }

        public IEnumerable<SparkDescriptor> ViewDescriptors()
        {
            return descriptors(t => t.IsSparkView()).ToList();
        }

        private IEnumerable<SparkDescriptor> descriptors(Func<ITemplate, bool> selector)
        {
            return this.Where(t => t.Descriptor is SparkDescriptor && selector(t))
                .Select(t => t.Descriptor.As<SparkDescriptor>());
        }
    }
}