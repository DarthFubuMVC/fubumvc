using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using Spark.FileSystem;

namespace FubuMVC.Spark.SparkModel
{
    public class TemplateViewFolder : IViewFolder
    {
        private readonly IEnumerable<ISparkTemplate> _templates;
        private readonly Cache<string, IList<string>> _listViews;
        private readonly Cache<string, bool> _hasView;
        private readonly Cache<string, FileSystemViewFile> _getViewSource;

        public TemplateViewFolder(IEnumerable<ISparkTemplate> templates)
        {
            _templates = templates;
            _listViews = new Cache<string, IList<string>>(listViews);
            _hasView = new Cache<string, bool>(hasView);
            _getViewSource = new Cache<string, FileSystemViewFile>(getViewSource);
        }

        public IList<string> ListViews(string path)
        {
            return _listViews[path ?? string.Empty];
        }

        public bool HasView(string path)
        {
            return _hasView[path];
        }

        public IViewFile GetViewSource(string path)
        {
            return _getViewSource[path];
        }

        private IList<string> listViews(string path)
        {
            return _templates
				.Where(x => x.ViewPath.DirectoryPath() == path)
				.Select(x => x.ViewPath)
				.ToList();
        }

        private bool hasView(string path)
        {
            return _templates.Any(x => x.ViewPath == path.Replace('\\', '/'));
        }

        private FileSystemViewFile getViewSource(string path)
        {
            var template = _templates
				.Where(x => x.ViewPath == path.Replace('\\', '/') || x.FilePath == path || x.RelativeDirectoryPath() == path)
				.Select(x => new FileSystemViewFile(x.FilePath))
				.FirstOrDefault();

            return template;
        }
    }
}