using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime.Files;

namespace FubuMVC.Core.View.Model
{
    public class ViewCollection
    {
    }

    public class ViewCollection<T> where T : ITemplateFile
    {
        private readonly IList<BottleViews<T>> _bottles = new List<BottleViews<T>>(); 

        // TODO -- this will need to use the fancier exclusions later
        public void Fill(IFubuApplicationFiles files, FileSet matching, Func<IFubuFile, T> builder)
        {
            files.AllFolders.Each(folder => {
                var views = folder.FindFiles(matching).Select(builder);
                var bottle = new BottleViews<T>(folder.Provenance, views);
                _bottles.Add(bottle);
            });
        }

        public IEnumerable<BottleViews<T>> Bottles
        {
            get { return _bottles; }
        }

        public IEnumerable<T> Views
        {
            get
            {
                return _bottles.SelectMany(x => x.Views);
            }
        } 
    }

    public class BottleViews<T> where T : ITemplateFile
    {
        private readonly string _provenance;
        private readonly Cache<string, T> _views = new Cache<string, T>();

        public BottleViews(string provenance, IEnumerable<T> views)
        {
            _provenance = provenance;
            views.Each(x => _views[x.RelativeDirectoryPath()] = x);
        }

        public string Provenance
        {
            get { return _provenance; }
        }

        public IEnumerable<T> Views
        {
            get { return _views; }
        }
    }
}