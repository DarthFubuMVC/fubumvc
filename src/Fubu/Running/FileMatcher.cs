using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;
using System.Linq;

namespace Fubu.Running
{
    public interface IFileMatcher
    {
        FileChangeCategory CategoryFor(string file);
    }

    public class FileMatcher : IFileMatcher
    {
        public static readonly string File = "file-patterns.txt";

        private readonly Cache<FileChangeCategory, IList<IFileMatch>> _matchers = new Cache<FileChangeCategory, IList<IFileMatch>>(x => new List<IFileMatch>());
        private readonly Cache<string, FileChangeCategory> _results;

        public static FileMatcher ReadFromFile(string file)
        {
            var system = new FileSystem();
            var matcher = new FileMatcher();

            system.ReadTextFile(file, text => {
                if (text.IsEmpty()) return;

                var match = Build(text);
                matcher.Add(match);
            });

            return matcher;
        }

        public FileMatcher()
        {
            _results = new Cache<string, FileChangeCategory>(file => {
                if (matches(FileChangeCategory.AppDomain, file)) return FileChangeCategory.AppDomain;
                if (matches(FileChangeCategory.Application, file)) return FileChangeCategory.Application;
                if (matches(FileChangeCategory.Content, file)) return FileChangeCategory.Content;
                return FileChangeCategory.Nothing;
            });

            Add(new BinFileMatch());
            Add(new ExactFileMatch(FileChangeCategory.AppDomain, "web.config"));
            Add(new ExtensionMatch(FileChangeCategory.AppDomain, "*.exe"));
            Add(new ExtensionMatch(FileChangeCategory.AppDomain, "*.dll"));
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

        public static IFileMatch Build(string text)
        {
            var parts = text.Split('=');
            var category = Enum.Parse(typeof (FileChangeCategory), parts.Last(), true)
                .As<FileChangeCategory>();
            var pattern = parts.First();

            if (!pattern.StartsWith("*"))
            {
                return new ExactFileMatch(category, pattern);
            }
            
            if (pattern.Split('.').Count() > 2)
            {
                return new EndsWithPatternMatch(category, pattern);
            }

            return new ExtensionMatch(category, pattern);
        }
    }
}