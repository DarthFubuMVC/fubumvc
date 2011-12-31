using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Razor.RazorModel
{
    public class TemplateRegistry : List<ITemplate>, ITemplateRegistry
    {
        public TemplateRegistry() {}
        public TemplateRegistry(IEnumerable<ITemplate> templates) : base(templates) { }

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
        IEnumerable<ITemplate> ByNameUnderDirectories(string name, IEnumerable<string> directories);
        IEnumerable<ITemplate> AllTemplates();
        IEnumerable<ITemplate> ByOrigin(string origin);
        IEnumerable<ITemplate> FromHost();
    }
}