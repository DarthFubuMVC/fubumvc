using System.IO;
using FubuCore;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    public interface ITemplateFile
    {
        string FilePath { get; set; }
        string RootPath { get; set; }
        string Origin { get; set; }
        string ViewPath { get; set; }
        ITemplateDescriptor Descriptor { get; set; }
        string RelativePath();
        string DirectoryPath();
        string RelativeDirectoryPath();
        string Name();
        bool FromHost();
        bool IsPartial();
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

        public string RelativePath()
        {
            return FilePath.PathRelativeTo(RootPath);
        }

        public string DirectoryPath()
        {
            return Path.GetDirectoryName(FilePath);
        }

        public string RelativeDirectoryPath()
        {
            return DirectoryPath().PathRelativeTo(RootPath);
        }

        public string Name()
        {
            return Path.GetFileNameWithoutExtension(FilePath);
        }

        public bool FromHost()
        {
            return Origin == ContentFolder.Application;
        }

        public bool IsPartial()
        {
            return Path.GetFileName(FilePath).StartsWith("_");
        }


    }
}