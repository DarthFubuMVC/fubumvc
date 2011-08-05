using System.Collections.Generic;

namespace FubuMVC.Core.Assets
{
    public abstract class AssetBase : IAsset
    {
        private readonly List<string> _aliases = new List<string>();
        private readonly List<IAsset> _dependencies = new List<IAsset>();

        public bool Matches(string key)
        {
            var keyToMatch = key.ToLowerInvariant();
            return Name.ToLowerInvariant() == keyToMatch || _aliases.Contains(keyToMatch);
        }

        public string Name { get; set; }

        public void AddAlias(string alias)
        {
            _aliases.Fill(alias.ToLowerInvariant());
        }

        public abstract IEnumerable<IAssetDependency> AllScripts();

        public IEnumerable<IAsset> Dependencies()
        {
            return _dependencies;
        }

        public void AddDependency(IAsset asset)
        {
            _dependencies.Fill(asset);
        }
    }
}