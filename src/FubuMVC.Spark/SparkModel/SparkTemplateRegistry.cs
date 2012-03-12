using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.View.Model;

namespace FubuMVC.Spark.SparkModel
{
    public interface ISparkTemplateRegistry : ITemplateRegistry<ITemplate>
    {
        IEnumerable<ITemplate> BindingsForView(string viewPath);
    }

    public class SparkTemplateRegistry : TemplateRegistry<ITemplate>, ISparkTemplateRegistry
    {
        public SparkTemplateRegistry()
        {
        }

        public SparkTemplateRegistry(IEnumerable<ITemplate> templates) : base(templates)
        {
        }

        public IEnumerable<ITemplate> BindingsForView(string viewPath)
        {
            return this
                .Where(x => x.ViewPath == viewPath && x.Descriptor is SparkDescriptor)
                .SelectMany(x => x.Descriptor.As<SparkDescriptor>().Bindings)
                .ToList();
        }
    }
}