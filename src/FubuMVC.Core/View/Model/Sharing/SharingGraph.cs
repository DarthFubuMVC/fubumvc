using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Runtime.Files;
using HtmlTags.Extended.Attributes;

namespace FubuMVC.Core.View.Model.Sharing
{
    [MarkedForTermination]
    public interface ISharingGraph
    {
        IEnumerable<string> SharingsFor(string provenance);
    }

    public class SharingGraph : ISharingRegistration, ISharingGraph
    {
        private readonly List<string> _globals = new List<string>();
        private readonly Cache<string, IList<string>> _dependencies = new Cache<string, IList<string>>(name => new List<string>()); 

        public void Global(string global)
        {
            _globals.Fill(global);
        }

        public void Dependency(string dependent, string dependency)
        {
            if(dependent == dependency) return;

            _dependencies[dependent].Fill(dependency);
        }

        public IEnumerable<string> SharingsFor(string provenance)
        {
            return findSharings(provenance);
        }

        private IEnumerable<string> findSharings(string provenance)
        {
            foreach (string name in _dependencies[provenance])
            {
                yield return name;
            }

            foreach (var global in _globals)
            {
                yield return global;
            }

            yield return ContentFolder.Application;
        } 

    }
}