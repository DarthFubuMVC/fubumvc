using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateFile
    {
        string Origin { get; }
        string FilePath { get; }
        string RootPath { get; }

        string ViewPath { get; set; }
        ITemplateDescriptor Descriptor { get; set; }
    }

    public class Template : ITemplateFile
    {
        private ITemplateDescriptor _descriptor = new NulloDescriptor();

        public Template(IFubuFile file)
            : this(file.Path, file.ProvenancePath, file.Provenance)
        {
        }

        public Template(string filePath, string rootPath, string origin)
        {
            FilePath = filePath;
            RootPath = rootPath;
            Origin = origin;
        }

        public string FilePath { get; set; }
        public string RootPath { get; set; }
        public string Origin { get; set; }

        public string ViewPath { get; set; }

        public ITemplateDescriptor Descriptor
        {
            get { return _descriptor; }
            set { _descriptor = value; }
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}