using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.Diagnostics;
using FubuCore.Util;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetGraph : IComparer<IFileDependency>, IAssetRegistration
    {
        private readonly Cache<string, IRequestedAsset> _objects = new Cache<string, IRequestedAsset>();
        private readonly Cache<string, AssetSet> _sets = new Cache<string, AssetSet>();
        private readonly List<Extension> _extenders = new List<Extension>();
        private readonly List<DependencyRule> _rules = new List<DependencyRule>();
        private readonly List<PreceedingAsset> _preceedings = new List<PreceedingAsset>();

        public AssetGraph()
        {
            _sets.OnMissing = name => new AssetSet{
                Name = name
            };

            _sets.OnAddition = (@set) =>
            {
                _objects[@set.Name] = @set;
            };


            _objects.OnMissing = name =>
            {
                return 
                    _objects.GetAll().FirstOrDefault(x => x.Matches(name)) 
                    ?? 
                    new FileDependency(name);
            };
        }

        public int Compare(IFileDependency x, IFileDependency y)
        {
            if (ReferenceEquals(x, y)) return 0;

            if (x.MustBeAfter(y)) return 1;
            if (y.MustBeAfter(x)) return -1;

            return 0;
        }

        public IEnumerable<IFileDependency> AllDependencies()
        {
            return _objects.OfType<IFileDependency>();
        }

        public void Alias(string name, string alias)
        {
            _objects[name].AddAlias(alias);
        }

        public void Dependency(string dependent, string dependency)
        {
            _rules.Fill(new DependencyRule()
                        {
                            Dependency = dependency,
                            Dependent = dependent
                        });
        }

        public void Extension(string extender, string @base)
        {
            _extenders.Add(new Extension(){
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
            _preceedings.Add(new PreceedingAsset(){
                Before = beforeName,
                After = afterName
            });
        }

        public IEnumerable<IFileDependency> GetAssets(IEnumerable<string> names)
        {
            return new AssetGatherer(this, names).Gather();
        }

        public AssetSet AssetSetFor(string someName)
        {
            return _sets[someName];
        }


        // TODO -- try to find circular dependencies and log to the Package log
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

    }
}