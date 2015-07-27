using System;
using System.IO;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.View.Rendering;

namespace FubuMVC.Core.View.Model
{
    public abstract class Template : ITemplateFile
    {
        private readonly Lazy<Parsing> _parsing;
        private ITemplateFile _master;

        public Template(IFubuFile file, IFubuApplicationFiles files)
        {
            _file = file;


            _parsing = new Lazy<Parsing>(() => createParsing(files));


            _relativeDirectoryPath = _file.RelativePath.ParentDirectory().Replace('\\', '/');
        }

        public Parsing Parsing
        {
            get { return _parsing.Value; }
        }

        protected abstract Parsing createParsing(IFubuApplicationFiles files);

        public string FilePath
        {
            get { return _file.Path; }
        }


        public string ViewPath
        {
            get { return _file.RelativePath; }
        }

        public override string ToString()
        {
            return FilePath;
        }

        public string RelativePath()
        {
            return _file.RelativePath;
        }

        public string DirectoryPath()
        {
            return Path.GetDirectoryName(FilePath);
        }

        private readonly string _relativeDirectoryPath;
        private readonly IFubuFile _file;

        public string RelativeDirectoryPath()
        {
            return _relativeDirectoryPath;
        }

        public string Name()
        {
            return Path.GetFileNameWithoutExtension(FilePath);
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


        // Tested through integration tests
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