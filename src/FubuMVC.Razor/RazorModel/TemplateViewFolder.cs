﻿using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Razor.FileSystem;

namespace FubuMVC.Razor.RazorModel
{
    public class TemplateViewFolder : IViewFolder
    {
        private readonly IEnumerable<ITemplate> _templates;
        private readonly Cache<string, IList<string>> _listViews;
        private readonly Cache<string, bool> _hasView;
        private readonly Cache<string, FileSystemViewFile> _getViewSource;

        public TemplateViewFolder(IEnumerable<ITemplate> templates)
        {
            _templates = templates;
            _listViews = new Cache<string, IList<string>>(listViews);
            _hasView = new Cache<string, bool>(hasView);
            _getViewSource = new Cache<string, FileSystemViewFile>(getViewSource);
        }

        public IList<string> ListViews(string path)
        {
            return _listViews[path];
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
            return _templates.Any(x => x.ViewPath == path);
        }

        private FileSystemViewFile getViewSource(string path)
        {
            return _templates
				.Where(x => x.ViewPath == path)
				.Select(x => new FileSystemViewFile(x.FilePath))
				.First();
        }
    }
}