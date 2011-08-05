using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.Assets
{
    public class AssetDependency : AssetBase, IAssetDependency
    {
        private readonly IList<IAssetDependency> _extensions = new List<IAssetDependency>();
        private readonly Cache<IAssetDependency, bool> _isAfter = new Cache<IAssetDependency, bool>();
        private bool _hasPreceeding;

        public AssetDependency()
        {
            _isAfter.OnMissing = isAfterInChain;
        }

        public AssetDependency(string name) : this()
        {
            Name = name;
        }

        public override IEnumerable<IAssetDependency> AllScripts()
        {
            yield return this;

            foreach (var extension in _extensions)
            {
                yield return extension;
            }
        }

        public bool MustBeAfter(IAssetDependency assetDependency)
        {
            var returnValue = _isAfter[assetDependency];
            return returnValue;
        }

        public void MustBePreceededBy(IAssetDependency assetDependency)
        {
            _hasPreceeding = true;
            _isAfter[assetDependency] = true;
        }

        public void AddExtension(IAssetDependency extender)
        {
            _extensions.Add(extender);
            _isAfter[extender] = false;
        }

        // TODO -- need unit test.  Well tested thru StoryTeller, but still
        public bool IsFirstRank()
        {
            return (!_hasPreceeding) && (!Dependencies().Any());
        }

        public int CompareTo(IAssetDependency other)
        {
            return Name.CompareTo(other.Name);
        }

        private bool isAfterInChain(IAssetDependency assetDependency)
        {
            // The filter on "not this" is introduced because of the extensions
            var dependencies = Dependencies().SelectMany(x => x.AllScripts()).Where(x => !ReferenceEquals(x, this));
            return dependencies.Contains(assetDependency) || dependencies.Any(x => x.MustBeAfter(assetDependency));
        }

        public override string ToString()
        {
            return string.Format("Script: {0}", Name);
        }
    }
}