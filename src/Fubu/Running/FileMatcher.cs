using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuCore.Util;
using System.Linq;
using FubuMVC.Core;

namespace Fubu.Running
{
    public interface IFileMatcher
    {
        FileChangeCategory CategoryFor(string file);
    }

    public class AnywhereAssetFileMatcher : IFileMatch
    {
        private readonly string[] _extensions;

        public AnywhereAssetFileMatcher(IEnumerable<string> extensions)
        {
            _extensions = extensions.Select(Path.GetExtension).ToArray();
        }

        public bool Matches(string file)
        {
            return _extensions.Contains(Path.GetExtension(file));
        }

        public FileChangeCategory Category
        {
            get
            {
                return FileChangeCategory.Content;
            }
        }
    }

    public class PublicFolderAssetFileMatcher : IFileMatch
    {
        private AnywhereAssetFileMatcher _inner;
        private string _directory;

        public PublicFolderAssetFileMatcher(string directory, IEnumerable<string> extensions)
        {
            _inner = new AnywhereAssetFileMatcher(extensions);
            _directory = directory;
        }

        public bool Matches(string file)
        {
            return file.StartsWith(_directory) && _inner.Matches(file);
        }

        public FileChangeCategory Category
        {
            get
            {
                return FileChangeCategory.Content;
                
            }
        }
    }

    public class FileMatcher : IFileMatcher
    {
        private readonly Cache<FileChangeCategory, IList<IFileMatch>> _matchers = new Cache<FileChangeCategory, IList<IFileMatch>>(x => new List<IFileMatch>());
        private readonly Cache<string, FileChangeCategory> _results;


        public FileMatcher(FileWatcherManifest manifest)
        {
            _results = new Cache<string, FileChangeCategory>(file => {
                if (matches(FileChangeCategory.AppDomain, file)) return FileChangeCategory.AppDomain;
                if (matches(FileChangeCategory.Application, file)) return FileChangeCategory.Application;
                if (matches(FileChangeCategory.Content, file)) return FileChangeCategory.Content;
                return FileChangeCategory.Nothing;
            });

            if (manifest.PublicAssetFolder.IsEmpty())
            {
                Add(new AnywhereAssetFileMatcher(manifest.AssetExtensions));
            }
            else
            {
                Add(new PublicFolderAssetFileMatcher(manifest.PublicAssetFolder, manifest.AssetExtensions));
            }

            Add(new ExactFileMatch(FileChangeCategory.AppDomain, manifest.ConfigurationFile));

            Add(new BinFileMatch());
            Add(new ExactFileMatch(FileChangeCategory.AppDomain, "web.config"));
            Add(new ExtensionMatch(FileChangeCategory.AppDomain, "*.exe"));
            Add(new ExtensionMatch(FileChangeCategory.AppDomain, "*.dll"));

            manifest.ContentMatches.Where(x => x.StartsWith("*."))
                .Each(x => Add(new ExtensionMatch(FileChangeCategory.Content, x)));

            manifest.ContentMatches.Where(x => !x.StartsWith("*."))
                .Each(x => Add(new ExactFileMatch(FileChangeCategory.Content, x)));


        }

        public IEnumerable<IFileMatch> MatchersFor(FileChangeCategory category)
        {
            return _matchers[category];
        } 

        public FileChangeCategory CategoryFor(string file)
        {
            return _results[file];
        }

        private bool matches(FileChangeCategory category, string file)
        {
            var matchers = _matchers[category];
            return matchers.Any(x => x.Matches(file));
        }

        public void Add(IFileMatch match)
        {
            _matchers[match.Category].Add(match);
        }

    }
}