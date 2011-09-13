using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public class TemplateRegistry : List<ITemplate>, ITemplateRegistry
    {
        public TemplateRegistry() {}
        public TemplateRegistry(IEnumerable<ITemplate> templates) : base(templates) { }

        public IEnumerable<ITemplate> BindingsForView(string viewPath)
        {
            return this
                .Where(x => x.ViewPath == viewPath && x.Descriptor is ViewDescriptor)
                .SelectMany(x => x.Descriptor.As<ViewDescriptor>().Bindings)
                .ToList();
        }

        public IEnumerable<ITemplate> ByNameUnderDirectories(string name, IEnumerable<string> directories)
        {
            return directories
                .SelectMany(local => this.Where(x => x.Name() == name && x.DirectoryPath() == local));
        }

        public ITemplate FirstByName(string name)
        {
            return this.Where(x => x.Name() == name).FirstOrDefault();
        }

        public IEnumerable<ITemplate> ByOrigin(string origin)
        {
            return this.Where(x => x.Origin == origin);
        }

        public IEnumerable<ITemplate> AllTemplates()
        {
            return this;
        }

        public IEnumerable<ITemplate> FromHost()
        {
            return this.Where(x => x.FromHost());
        }
    }

    public interface ITemplateRegistry
    {
        IEnumerable<ITemplate> BindingsForView(string viewPath);
        IEnumerable<ITemplate> ByNameUnderDirectories(string name, IEnumerable<string> directories);
        IEnumerable<ITemplate> AllTemplates();
        IEnumerable<ITemplate> ByOrigin(string origin);
        IEnumerable<ITemplate> FromHost();
    }
}