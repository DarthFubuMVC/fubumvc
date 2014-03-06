using System;
using System.IO;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    public abstract class Template : ITemplateFile
    {
        private static readonly Cache<string, string> _cache = new Cache<string, string>(getPrefix);

        private static string getPrefix(string origin)
        {
            return origin == ContentFolder.Application ? string.Empty : "_{0}".ToFormat(origin);
        }

        private ITemplateDescriptor _descriptor = new NulloDescriptor();
        private Lazy<Parsing> _parsing;
        private ITemplateFile _master;

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

            _parsing = new Lazy<Parsing>(createParsing);
        }

        public Parsing Parsing
        {
            get { return _parsing.Value; }
        }

        protected abstract Parsing createParsing();

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

        public void AttachViewModels(ViewTypePool types, ITemplateLogger logger)
        {
            if (IsPartial()) return;

            var typeName = Parsing.ViewModelType;
            if (typeName.IsEmpty()) return;

            ViewModel = types.FindTypeByName(typeName, message => logger.Log(this, message));
        }

        public ITemplateFile Master
        {
            get { return _master; }
            set
            {
                if (value != null && value.GetType() != GetType())
                {
                    throw new ArgumentOutOfRangeException("value", "Mismatch in template types between {0} and {1}".ToFormat(value.GetType().FullName, GetType().FullName));
                }
                
                _master = value;
            }
        }
    }
}