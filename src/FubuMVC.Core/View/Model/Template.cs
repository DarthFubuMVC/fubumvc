using System;
using System.IO;
using System.Reflection;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.View.Model
{
    public abstract class Template : ITemplateFile
    {
        private static readonly Cache<string, string> _cache = new Cache<string, string>(getPrefix);


        private static string getPrefix(string origin)
        {
            return origin == ContentFolder.Application ? string.Empty : "_{0}".ToFormat(origin);
        }

        private readonly Lazy<Parsing> _parsing;
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

            ViewPath = FileSystem.Combine(_cache[Origin], RelativePath()).Replace('\\', '/');

            _parsing = new Lazy<Parsing>(createParsing);

            _relativeDirectoryPath = new Lazy<string>(() => DirectoryPath().PathRelativeTo(RootPath).Replace('\\', '/'));
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

        public override string ToString()
        {
            return FilePath;
        }

        public string RelativePath()
        {
            return FilePath.PathRelativeTo(RootPath).Replace("\\", "/");
        }

        public string DirectoryPath()
        {
            return Path.GetDirectoryName(FilePath);
        }

        private readonly Lazy<string> _relativeDirectoryPath;

        public string RelativeDirectoryPath()
        {
            return _relativeDirectoryPath.Value;
        }

        public string Name()
        {
            return Path.GetFileNameWithoutExtension(FilePath);
        }

        public bool FromHost()
        {
            return Origin == ContentFolder.Application;
        }

        public virtual bool IsPartial()
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

        // Tested through integration tests
        public void AttachViewModels(Assembly defaultAssembly, ViewTypePool types, ITemplateLogger logger)
        {
            if (IsPartial()) return;

            var typeName = Parsing.ViewModelType;
            if (typeName.IsEmpty()) return;

            ViewModel = types.FindTypeByName(typeName, defaultAssembly, message => logger.Log(this, message));
        }

        public ITemplateFile Master
        {
            get { return _master; }
            set
            {
                if (value != null && ReferenceEquals(value, this)) return;

                if (value != null && value.GetType() != GetType())
                {
                    throw new ArgumentOutOfRangeException("value",
                        "Mismatch in template types between {0} and {1}".ToFormat(value.GetType().FullName,
                            GetType().FullName));
                }

                // This prevents Stackoverflow problems
                if (value != null && value.Parsing.Master == Name()) return;



                _master = value;
            }
        }

        public string ProfileName { get; set; }

        public abstract IRenderableView GetView();
        public abstract IRenderableView GetPartialView();

        public void AttachLayouts(string defaultLayoutName, IViewFacility facility, ITemplateFolder folder)
        {
            if (IsPartial()) return;
            if (Master != null) return;

            if (Parsing.Master == string.Empty) return;

            var layoutName = Parsing.Master.IsEmpty() ? defaultLayoutName : Parsing.Master;

            if (layoutName.EqualsIgnoreCase("none")) return;

            if (layoutName == Name()) return;



            Master = folder.FindRecursivelyInShared(layoutName)
                ?? facility.FindInShared(layoutName);

        }
    }
}