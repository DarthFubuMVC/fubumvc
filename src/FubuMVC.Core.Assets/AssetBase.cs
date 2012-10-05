using System.Collections.Generic;

namespace FubuMVC.Core.Assets
{
    public abstract class AssetBase : IRequestedAsset
    {
        private readonly List<string> _aliases = new List<string>();
        private readonly List<IRequestedAsset> _dependencies = new List<IRequestedAsset>();

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

        public abstract IEnumerable<IFileDependency> AllFileDependencies();

        public IEnumerable<IRequestedAsset> Dependencies()
        {
            return _dependencies;
        }

        public void AddDependency(IRequestedAsset asset)
        {
            _dependencies.Fill(asset);
        }
    }
}