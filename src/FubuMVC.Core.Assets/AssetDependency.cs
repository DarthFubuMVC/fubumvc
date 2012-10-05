using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;

namespace FubuMVC.Core.Assets
{
    public class FileDependency : AssetBase, IFileDependency
    {
        private readonly IList<IFileDependency> _extensions = new List<IFileDependency>();
        private readonly Cache<IFileDependency, bool> _isAfter = new Cache<IFileDependency, bool>();
        private bool _hasPreceeding;

        public FileDependency()
        {
            _isAfter.OnMissing = isAfterInChain;
        }

        public FileDependency(string name) : this()
        {
            Name = name;
        }

        public override IEnumerable<IFileDependency> AllFileDependencies()
        {
            yield return this;

            foreach (var extension in _extensions)
            {
                yield return extension;
            }
        }

        public bool MustBeAfter(IFileDependency fileDependency)
        {
            var returnValue = _isAfter[fileDependency];
            return returnValue;
        }

        public void MustBePreceededBy(IFileDependency fileDependency)
        {
            _hasPreceeding = true;
            _isAfter[fileDependency] = true;
        }

        public void AddExtension(IFileDependency extender)
        {
            _extensions.Add(extender);
            _isAfter[extender] = false;
        }

        public bool IsFirstRank()
        {
            return (!_hasPreceeding) && (!Dependencies().Any());
        }

        public int CompareTo(IFileDependency other)
        {
            return Name.CompareTo(other.Name);
        }

        private bool isAfterInChain(IFileDependency fileDependency)
        {
            // The filter on "not this" is introduced because of the extensions
            var dependencies = Dependencies().SelectMany(x => x.AllFileDependencies()).Where(x => !ReferenceEquals(x, this));
            return dependencies.Contains(fileDependency) || dependencies.Any(x => x.MustBeAfter(fileDependency));
        }

        public override string ToString()
        {
            return string.Format("Script: {0}", Name);
        }
    }
}