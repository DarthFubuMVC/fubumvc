using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Caching;
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
        public void Fill(IFubuApplicationFiles files, FileSet matching, Func<IFubuFile, T> builder)
        {
            
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
    }
}