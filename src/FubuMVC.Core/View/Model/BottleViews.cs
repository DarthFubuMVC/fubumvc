using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    // Tested through integration tests only
    public class BottleViews<T> where T : ITemplateFile
    {
        private readonly ViewFacility<T> _facility;
        private readonly ContentFolder _content;
        private readonly Func<IFubuFile, T> _builder;
        private readonly ViewEngineSettings _settings;
        private readonly FileSet _match;
        private readonly IList<ViewFolder<T>> _folders = new List<ViewFolder<T>>();
        private ViewFolder<T> _top;

        public BottleViews(ViewFacility<T> facility, ContentFolder content, Func<IFubuFile, T> builder, ViewEngineSettings settings, FileSet match)
        {
            if (match == null) throw new ArgumentNullException("match");
            _facility = facility;
            _content = content;
            _builder = builder;
            _settings = settings;
            _match = match;

            _top = buildFolder(content.Path);
        }


        private ViewFolder<T> buildFolder(string path)
        {
            var views = ViewEngineSettings.FileSystem.FindFiles(path, _match).Select(x => new FubuFile(x, Provenance)
            {
                ProvenancePath = _content.Path
            })
                .Select(_builder).Where(x => !_settings.IsExcluded(x));

            var folder = new ViewFolder<T>(Path.GetFileNameWithoutExtension(path)) {IsShared = _settings.IsSharedFolder(path)};
            folder.Views.AddRange(views);

            _folders.Add(folder);


            ViewEngineSettings.FileSystem.ChildDirectoriesFor(path).Each(child =>
            {
                var childFolder = buildFolder(child);
                childFolder.Parent = folder;

                if (childFolder.IsShared)
                {
                    folder.LayoutFolders.Add(childFolder);
                }
            });

            return folder;
        }

        public void AttachLayouts(string applicationLayoutName)
        {
            _folders.Each(x => x.AttachLayouts(applicationLayoutName, _facility));
        }


        public string Provenance
        {
            get { return _content.Provenance; }
        }

        public IEnumerable<T> AllViews()
        {
            return _folders.SelectMany(x => x.Views);
        }

        public T FindInShared(string viewName)
        {
            return _top.FindInShared(viewName);
        }

    }
}