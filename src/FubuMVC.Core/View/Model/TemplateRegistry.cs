using System.Collections.Generic;
using System.Linq;

namespace FubuMVC.Core.View.Model
{
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
            return this.Where(x => x.Name() == name).FirstOrDefault();
        }

        public IEnumerable<T> ByOrigin(string origin)
        {
            return this.Where(x => x.Origin == origin);
        }

        public IEnumerable<T> AllTemplates()
        {
            return this;
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