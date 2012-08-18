using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets
{
    public class AssetGraph : IComparer<IFileDependency>, IAssetRegistration
    {
        private readonly Cache<string, IList<string>> _combos =
            new Cache<string, IList<string>>(key => new List<string>());

        private readonly Cache<AssetFilesKey, IEnumerable<IFileDependency>> _dependencyCache;

        private readonly List<Extension> _extenders = new List<Extension>();
        private readonly Cache<string, IRequestedAsset> _objects = new Cache<string, IRequestedAsset>();
        private readonly IList<Type> _policyTypes = new List<Type>();
        private readonly List<PreceedingAsset> _preceedings = new List<PreceedingAsset>();
        private readonly List<DependencyRule> _rules = new List<DependencyRule>();
        private readonly Cache<string, AssetSet> _sets = new Cache<string, AssetSet>();
        private readonly IList<Action<IAssetRegistration>> _precompileActions = new List<Action<IAssetRegistration>>();

        public AssetGraph()
        {
            _sets.OnMissing = name => new AssetSet{
                Name = name
            };

            _sets.OnAddition = @set => { _objects[@set.Name] = @set; };


            _objects.OnMissing = name =>
            {
                return
                    _objects.GetAll().FirstOrDefault(x => x.Matches(name))
                    ??
                    new FileDependency(name);
            };

            _dependencyCache = new Cache<AssetFilesKey, IEnumerable<IFileDependency>>(key => new AssetGatherer(this, key.Names).Gather());
        }

        public void ForEachSetName(Action<string> action)
        {
            _sets.GetAllKeys().Each(action);
        }

        public IEnumerable<string> CorrectForAliases(IEnumerable<string> names)
        {
            return names.Select(x => _objects[x].Name);
        }

        public IList<Type> PolicyTypes
        {
            get { return _policyTypes; }
        }

        public void Alias(string name, string alias)
        {
            _objects[name].AddAlias(alias);
        }

        public void Dependency(string dependent, string dependency)
        {
            _rules.Fill(new DependencyRule{
                Dependency = dependency,
                Dependent = dependent
            });
        }

        public void Extension(string extender, string @base)
        {
            _extenders.Add(new Extension{
                Base = @base,
                Extender = extender
            });
        }

        public void AddToSet(string setName, string name)
        {
            _sets[setName].Add(name);
        }

        public void Preceeding(string beforeName, string afterName)
        {
            _preceedings.Add(new PreceedingAsset{
                Before = beforeName,
                After = afterName
            });
        }

        public void AddToCombination(string comboName, string names)
        {
            var unaliasedNames = CorrectForAliases(names.ToDelimitedArray()).ToList();
            verifySingleMimeTypeForCombination(comboName, unaliasedNames);
            _combos[comboName].Fill(unaliasedNames);
        }

        private void verifySingleMimeTypeForCombination(string comboName, IEnumerable<string> fileNames)
        {
            var mimeTypes = new HashSet<MimeType>(fileNames.Select(MimeType.MimeTypeByFileName));

            var combo = _combos[comboName];
            if (combo.Any())
            {
                mimeTypes.Add(MimeType.MimeTypeByFileName(combo[0]));
            }

            if (mimeTypes.Count > 1)
            {
                throw new InvalidOperationException("All members of a combination must be of the same type (script or stylesheet)");
            }
        }

        public void ApplyPolicy(string typeName)
        {
            var type = Type.GetType(typeName, false);
            if (type == null)
            {
                var comboType = typeof (CombineAllScriptFiles);
                var tryName = "{0}.{1},{2}".ToFormat(comboType.Namespace, typeName, comboType.Assembly.GetName().Name);

                type = Type.GetType(tryName);
            }

            if (type == null)
            {
                throw new ArgumentOutOfRangeException("Type {0} cannot be found".ToFormat(typeName));
            }

            _policyTypes.Fill(type);
        }

        public int Compare(IFileDependency x, IFileDependency y)
        {
            if (ReferenceEquals(x, y)) return 0;

            if (x.MustBeAfter(y)) return 1;
            if (y.MustBeAfter(x)) return -1;

            return 0;
        }

        /// <summary>
        ///   Use this method in automated tests when you need to set up an
        ///   AssetGraph
        /// </summary>
        /// <param name = "configure"></param>
        /// <returns></returns>
        public static AssetGraph Build(Action<AssetGraph> configure)
        {
            var graph = new AssetGraph();
            configure(graph);
            graph.CompileDependencies(new PackageLog());

            return graph;
        }

        public IEnumerable<IFileDependency> AllDependencies()
        {
            return _objects.OfType<IFileDependency>();
        }

        public void ForCombinations(Action<string, IList<string>> combinations)
        {
            _combos.Each(combinations);
        }

        public IEnumerable<string> NamesForCombination(string comboName)
        {
            return _combos[comboName];
        }

        public IEnumerable<IFileDependency> GetAssets(IEnumerable<string> names)
        {
            return _dependencyCache[new AssetFilesKey(names)];
        }

        public AssetSet AssetSetFor(string someName)
        {
            return _sets[someName];
        }


        public void CompileDependencies(IPackageLog log)
        {
            _sets.Each(set => set.FindScripts(this));
            _rules.Each(rule =>
            {
                var dependency = ObjectFor(rule.Dependency);
                var dependent = ObjectFor(rule.Dependent);

                dependency.AllFileDependencies().Each(dependent.AddDependency);
            });

            _extenders.Each(x =>
            {
                var @base = FileDependencyFor(x.Base);
                var extender = FileDependencyFor(x.Extender);

                @base.AddExtension(extender);
                extender.AddDependency(@base);
            });

            _preceedings.Each(x =>
            {
                var before = FileDependencyFor(x.Before);
                var after = FileDependencyFor(x.After);

                after.MustBePreceededBy(before);
            });
        }

        // Find by name or by alias
        public IRequestedAsset ObjectFor(string name)
        {
            return _objects[name];
        }

        public IFileDependency FileDependencyFor(string name)
        {
            return (IFileDependency) ObjectFor(name);
        }

        public void OnPrecompile(Action<IAssetRegistration> action)
        {
            _precompileActions.Add(action);
        }

        public void Precompile()
        {
            _precompileActions.Each(x => x(this));
        }

        #region Nested type: AssetFilesKey

        public class AssetFilesKey
        {
            private readonly List<string> _names;

            public AssetFilesKey(IEnumerable<string> names)
            {
                _names = names.ToList();
                _names.Sort();
            }

            public List<string> Names
            {
                get { return _names; }
            }

            public bool Equals(AssetFilesKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;

                if (other._names.Count != _names.Count) return false;

                return !other._names.Where((t, i) => t != _names[i]).Any();
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (AssetFilesKey)) return false;
                return Equals((AssetFilesKey) obj);
            }

            public override int GetHashCode()
            {
                return (_names != null ? _names.Join("-").GetHashCode() : 0);
            }
        }

        #endregion
    }
}