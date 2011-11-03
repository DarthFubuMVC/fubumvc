using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Runtime;
using FubuCore;

namespace Serenity.Jasmine
{
    public class Specification
    {
        private readonly string _contentFolder;
        private readonly string _subject;
        private readonly static IList<string> _ignoredExtensions = new List<string>();
        private readonly IList<AssetFile> _libraries = new List<AssetFile>();

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

        public static bool IsSpecification(AssetFile file)
        {
            return file.MimeType == MimeType.Javascript && file.ContentFolder().IsNotEmpty() && file.ContentFolder().Split('/').Contains("specs");
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

        public bool DependsOn(AssetFile other)
        {
            if (!string.Equals(_contentFolder, other.ContentFolder(), StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return string.Equals(_subject, DetermineLibraryName(other), StringComparison.InvariantCultureIgnoreCase);
        }

        public void AddLibrary(AssetFile file)
        {
            _libraries.Add(file);
        }

        public IEnumerable<AssetFile> Libraries
        {
            get { return _libraries; }
        }
    }
}