using System;
using System.Collections.Generic;
using FubuMVC.Core.Assets.Files;
using System.Linq;
using FubuMVC.Core.Runtime;

namespace Serenity.Jasmine
{
    public class Specification
    {
        private readonly string _contentFolder;
        private readonly string _subject;
        private readonly static IList<string> _ignoredExtensions = new List<string>();

        static Specification()
        {
            RebuildIgnoredExtensions();
        }

        public static IEnumerable<string> IgnoredExtensions
        {
            get { return _ignoredExtensions; }
        }

        public static void IgnoreExtension(string extension)
        {
            _ignoredExtensions.Add(extension);
        }

        public static void RebuildIgnoredExtensions()
        {
            _ignoredExtensions.Clear();

            _ignoredExtensions.AddRange(MimeType.Javascript.Extensions);
            _ignoredExtensions.Add(".spec");
        }

        public static string DetermineLibraryName(AssetFile file)
        {
            var libraryNameParts = file.LibraryName().Split('.').ToList();
            while (libraryNameParts.Any() && _ignoredExtensions.Contains("." + libraryNameParts.Last()))
            {
                libraryNameParts.RemoveAt(libraryNameParts.Count - 1);
            }

            return libraryNameParts.Join(".");
        }

        public Specification(string name) : this(new AssetFile(name))
        {

        }

        public Specification(AssetFile file)
        {
            File = file;

            string fileContentFolder = file.ContentFolder();
            if (fileContentFolder == null || fileContentFolder == "specs")
            {
                _contentFolder = null;
            }
            else
            {
                var list = fileContentFolder.Split('/').ToList();
                list.Remove("specs");

                _contentFolder = list.Join("/");
            }

            var libraryName = file.LibraryName();
            var libraryParts = libraryName.Split('.').ToList();
            var index = libraryParts.IndexOf("spec");


            _subject = libraryParts.Take(index).Join(".");
        }

        public string Subject
        {
            get { return _subject; }
        }


        public string ContentFolder
        {
            get { return _contentFolder; }
        }

        public AssetFile File { get; private set;}
        public AssetPath Path { get; set;}

        public bool DependsOn(AssetFile other)
        {
            if (!string.Equals(_contentFolder, other.ContentFolder(), StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return string.Equals(_subject, DetermineLibraryName(other), StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class SpecificationGraph
    {
        
    }

    public class SpecificationFolder
    {
        private readonly IList<Specification> _specifications = new List<Specification>();
        private readonly IList<SpecificationFolder> _children = new List<SpecificationFolder>();

        public void AddSpec(AssetFile file, AssetPath path)
        {
            _specifications.Add(new Specification(file){
                Path = path
            });
        }

        public IEnumerable<Specification> Specifications
        {
            get { return _specifications; }
        }

        public IEnumerable<Specification> AllSpecifications
        {
            get
            {
                return _specifications.Union(_children.SelectMany(x => x.Specifications));
            }
        }
    }
}