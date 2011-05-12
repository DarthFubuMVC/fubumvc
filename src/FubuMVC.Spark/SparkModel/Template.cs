using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Spark.SparkModel
{
    public interface ITemplate
    {
        string FilePath { get; }
        string RootPath { get; }
        string Origin { get; }
		
        string ViewPath { get; set; }
        ISparkDescriptor Descriptor { get; set; }
    }

    public class Template : ITemplate
    {
        public Template(string filePath, string rootPath, string origin)
        {
            FilePath = filePath;
            RootPath = rootPath;
            Origin = origin;
            Descriptor = new NulloDescriptor();
        }

        public string FilePath { get; private set; }
        public string RootPath { get; private set; }
        public string Origin { get; private set; }
		
        public string ViewPath { get; set; }
        public ISparkDescriptor Descriptor { get; set; }

	    public override string ToString()
        {
            return FilePath;
        }
    }

    public class Templates : List<ITemplate>, ITemplates
    {
        public Templates() {}
        public Templates(IEnumerable<ITemplate> templates) : base(templates) { }

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
    }

    public interface ITemplates 
    {
        IEnumerable<ITemplate> BindingsForView(string viewPath);
        ITemplate FirstByName(string name);
        IEnumerable<ITemplate> ByNameUnderDirectories(string name, IEnumerable<string> directories);
        IEnumerable<ITemplate> ByOrigin(string origin);
        IEnumerable<ITemplate> AllTemplates();
    }
}