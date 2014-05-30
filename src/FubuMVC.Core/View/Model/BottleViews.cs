using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    // Tested through integration tests only
    public class BottleViews<T> where T : class, ITemplateFile
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

            var folder = new ViewFolder<T>(path) {IsShared = _settings.IsSharedFolder(path)};
            folder.RelativePath = string.Empty;
            folder.Views.AddRange(views);

            _folders.Add(folder);


            ViewEngineSettings.FileSystem.ChildDirectoriesFor(path).Where(x => !_settings.FolderShouldBeIgnored(x)).Each(child =>
            {
                var childFolder = buildFolder(child);
                childFolder.Parent = folder;
                childFolder.RelativePath = (folder.RelativePath + "/" + childFolder.Name).TrimStart('/');

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

        public void AttachViewModels(ViewTypePool types, ITemplateLogger logger)
        {
            var assembly = types.FindAssemblyByProvenance(Provenance);
            AllViews().Each(x => x.AttachViewModels(assembly, types, logger));
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

        public IEnumerable<string> SharedPaths()
        {
            return _folders.Where(x => x.IsShared).Select(x => x.RelativePath);
        }
    }
}