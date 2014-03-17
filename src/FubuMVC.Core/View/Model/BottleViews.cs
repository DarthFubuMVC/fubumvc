using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    public class BottleViews<T> where T : ITemplateFile
    {
        private readonly ContentFolder _content;
        private readonly Func<IFubuFile, T> _builder;
        private readonly ViewEngines _viewEngines;
        private readonly FileSet _match;
        private readonly IList<ViewFolder<T>> _folders = new List<ViewFolder<T>>(); 

        public BottleViews(ContentFolder content, Func<IFubuFile, T> builder, ViewEngines viewEngines, FileSet match)
        {
            if (match == null) throw new ArgumentNullException("match");
            _content = content;
            _builder = builder;
            _viewEngines = viewEngines;
            _match = match;

            buildFolder(content.Path);
        }


        private ViewFolder<T> buildFolder(string path)
        {
            var views = ViewEngines.FileSystem.FindFiles(path, _match).Select(x => new FubuFile(x, Provenance)
            {
                ProvenancePath = StringExtensions.PathRelativeTo(x, _content.Path)
            })
                .Select(_builder).Where(x => !_viewEngines.IsExcluded(x));

            var folder = new ViewFolder<T> {IsShared = _viewEngines.IsSharedFolder(path)};
            folder.Views.AddRange(views);

            _folders.Add(folder);


            ViewEngines.FileSystem.ChildDirectoriesFor(path).Each(child =>
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


        public string Provenance
        {
            get { return _content.Provenance; }
        }

        public IEnumerable<T> AllViews()
        {
            return _folders.SelectMany(x => x.Views);
        } 

    }
}