using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FubuCore.Util;

namespace FubuMVC.Core.Assets
{
    public interface IAssetDependencyFinder
    {
        IEnumerable<string> CompileDependenciesAndOrder(IEnumerable<string> names);
    }

    public class AssetDependencyFinderCache : IAssetDependencyFinder
    {
        private readonly AssetGraph _graph;
        private readonly Cache<AssetNamesKey, IEnumerable<string>> _dependencies;

        public AssetDependencyFinderCache(AssetGraph graph)
        {
            _graph = graph;
            _dependencies = new Cache<AssetNamesKey, IEnumerable<string>>(FindDependencies);
        }

        public IEnumerable<string> CompileDependenciesAndOrder(IEnumerable<string> names)
        {
            return _dependencies[new AssetNamesKey(names)];
        }

        public virtual IEnumerable<string> FindDependencies(AssetNamesKey key)
        {
            return _graph.GetAssets(key.Names).Select(x => x.Name);
        }
    }

    public class AssetNamesKey
    {
        private readonly IEnumerable<string> _names;
        private readonly Lazy<int> _hashcode;

        public AssetNamesKey(IEnumerable<string> names)
        {
            _names = names.OrderBy(x => x);

            _hashcode = new Lazy<int>(() =>
            {
                var combined = names.Join("*");
                var encodedStream = Encoding.UTF8.GetBytes(combined);

                return MD5
                    .Create()
                    .ComputeHash(encodedStream)
                    .Select(b => b.ToString("x2"))
                    .Join("")
                    .GetHashCode();
            });
        }

        public IEnumerable<string> Names
        {
            get { return _names; }
        }

        public bool Equals(AssetNamesKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return other._names.IsEqualTo(_names);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AssetNamesKey)) return false;
            return Equals((AssetNamesKey) obj);
        }

        public override int GetHashCode()
        {
            return _hashcode.Value;
        }
    }


}