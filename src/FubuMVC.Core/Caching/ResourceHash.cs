using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Caching
{
    public interface IResourceHash
    {
        string CreateHash();
    }

    public class ResourceHash : IResourceHash
    {
        private readonly IEnumerable<IVaryBy> _varyBys;

        public ResourceHash(IEnumerable<IVaryBy> varyBys)
        {
            _varyBys = varyBys;
        }

        public static string For(params IVaryBy[] varyBys)
        {
            return new ResourceHash(varyBys).CreateHash();
        }

        public string CreateHash()
        {
            return _varyBys.SelectMany(x => x.Values()).Select(pair => "{0}={1}".ToFormat(pair.Key, pair.Value))
                .Join("&").ToHash();
        }
    }
}