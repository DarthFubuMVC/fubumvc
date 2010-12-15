using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FubuMVC.Core.Registration
{
    public class TypePool
    {
        private readonly Assembly _defaultAssembly;
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly IList<Type> _types = new List<Type>();
        private bool _scanned;
        private bool _ignoreCallingAssembly;
        private readonly IList<Func<IEnumerable<Assembly>>> _sources = new List<Func<IEnumerable<Assembly>>>();

        public TypePool(Assembly defaultAssembly)
        {
            _defaultAssembly = defaultAssembly;
        }

        public void IgnoreCallingAssembly()
        {
            _ignoreCallingAssembly = true;
        }

        public void AddSource(Func<IEnumerable<Assembly>> source)
        {
            _sources.Add(source);
        }

        private IList<Type> types
        {
            get
            {
                if (!_scanned && ShouldScanAssemblies)
                {
                    _scanned = true;

                    // TODO:  Good exception message when an assembly blows up on 
                    // GetExportedTypes()
                    _types.AddRange(Assemblies.SelectMany(x => x.GetExportedTypes()));
                }


                return _types;
            }
        }

        public bool ShouldScanAssemblies { get; set; }




        public void AddAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
        }

        public void AddType(Type type)
        {
            _types.Fill(type);
        }

        public void AddType<T>()
        {
            AddType(typeof (T));
        }

        public IEnumerable<Assembly> Assemblies
        {
            get
            {
                _assemblies.AddRange(_sources.SelectMany(x => x()));

                if (_assemblies.Any() == false && !_ignoreCallingAssembly)
                {
                    yield return _defaultAssembly;
                }

                foreach (var assembly in _assemblies.Distinct())
                {
                    yield return assembly;
                }
            }
        }

        // TODO -- diagnostics on type discovery!!!!
        public IEnumerable<Type> TypesMatching(Func<Type, bool> filter)
        {
            return types.Where(filter).Distinct();
        }

        public bool HasAssembly(Assembly assembly)
        {
            return _assemblies.Contains(assembly);
        }

        public void AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies.AddRange(assemblies);
        }
    }
}