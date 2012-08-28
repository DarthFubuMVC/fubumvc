using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.View.Model
{
    // TODO: Reconsider this
    public class TemplateRegistry<T> : List<T>, ITemplateRegistry<T> where T : ITemplateFile
    {
        public TemplateRegistry() {}
        public TemplateRegistry(IEnumerable<T> templates) : base(templates) { }

        public IEnumerable<T> ByNameUnderDirectories(string name, IEnumerable<string> directories)
        {
            return directories
                .SelectMany(local => this.Where(x => x.Name() == name && x.DirectoryPath() == local));
        }

        public T FirstByName(string name)
        {
            return this.FirstOrDefault(x => x.Name() == name);
        }

        public IEnumerable<T> ByOrigin(string origin)
        {
            return this.Where(x => x.Origin == origin);
        }

        public IEnumerable<T> AllTemplates()
        {
            return this;
        }

        // TODO: UT
        public IEnumerable<TDescriptor> DescriptorsWithViewModels<TDescriptor>() where TDescriptor : ViewDescriptor<T>
        {
            return AllTemplates().Where(t => t.Descriptor is TDescriptor)
                .Select(x => x.Descriptor.As<TDescriptor>())
                .Where(x => x.HasViewModel());
        }

        public IEnumerable<T> FromHost()
        {
            return this.Where(x => x.FromHost());
        }
    }

    public interface ITemplateRegistry<T> : IEnumerable<T>
    {
        IEnumerable<T> ByNameUnderDirectories(string name, IEnumerable<string> directories);
        IEnumerable<T> AllTemplates();
        IEnumerable<T> ByOrigin(string origin);
        IEnumerable<T> FromHost();
    }
}