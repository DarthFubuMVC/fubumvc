using System;
using System.IO;
using FubuCore;
using FubuCore.Util;
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
        string Namespace { get;  }
        Type ViewModel { get; set; }
        string RelativePath();
        string DirectoryPath();
        string RelativeDirectoryPath();
        string Name();
        bool FromHost();
        bool IsPartial();
        string FullName();
    }

    public class Template : ITemplateFile
    {
        private static readonly Cache<string, string> _cache = new Cache<string, string>(getPrefix);

        private static string getPrefix(string origin)
        {
            return origin == ContentFolder.Application ? string.Empty : "_{0}".ToFormat(origin);
        }

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

            ViewPath = FileSystem.Combine(_cache[Origin], RelativePath());
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

        public string Namespace
        {
            get
            {
                if (ViewModel == null) return string.Empty;

                var nspace = ViewModel.Assembly.GetName().Name;
                var relativePath = RelativePath();
                var relativeNamespace = Path.GetDirectoryName(relativePath);

                if (relativeNamespace.IsNotEmpty())
                {
                    nspace += "." + relativeNamespace.Replace(Path.DirectorySeparatorChar, '.');
                }

                return nspace;
            }
        }

        public Type ViewModel { get; set; }

        public string FullName()
        {
            return Namespace.IsEmpty() ? Name() : Namespace + "." + Name();
        }

        
    }
}