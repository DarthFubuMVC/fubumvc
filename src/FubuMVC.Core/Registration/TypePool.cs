using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// Bundle several assemblies and types to be treated as a single pool of types which can be
    /// queried and iterated over.
    /// </summary>
    public class TypePool
    {
        private readonly Assembly _defaultAssembly;
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly IList<Type> _types = new List<Type>();
        private bool _scanned;
        private bool _ignoreCallingAssembly;
        private readonly IList<Func<IEnumerable<Assembly>>> _sources = new List<Func<IEnumerable<Assembly>>>();
        

        /// <summary>
        /// Construct a type pool
        /// </summary>
        /// <param name="defaultAssembly">The default assembly, typically the calling assembly</param>
        public TypePool(Assembly defaultAssembly)
        {
            _defaultAssembly = defaultAssembly;
            IgnoreExportTypeFailures = true;
        }

        public bool IgnoreExportTypeFailures { get; set; }

        /// <summary>
        /// Ignore the assembly provided as default assemlbly in the constructor when enumerating assemblies
        /// of this type pool
        /// </summary>
        public void IgnoreCallingAssembly()
        {
            _ignoreCallingAssembly = true;
        }

        /// <summary>
        /// Register a function as a source of assemblies
        /// </summary>
        public void AddSource(Func<IEnumerable<Assembly>> source)
        {
            _sources.Add(source);
        }

        private IEnumerable<Type> types
        {
            get
            {
                if (!_scanned)
                {
                    _scanned = true;




                    // TODO:  Good exception message when an assembly blows up on 
                    // GetExportedTypes()
                    _types.AddRange(Assemblies.SelectMany(x =>
                    {
                        try
                        {
                            return x.GetExportedTypes();    
                        }
                        catch (Exception ex)
                        {
                            if (IgnoreExportTypeFailures)
                            {
                                return new Type[0];
                            }
                            else
                            {
                                throw new ApplicationException("Unable to find exported types from assembly " + x.FullName, ex);
                            }

                            
                            
                        }


                        
                    }));
                }


                return _types;
            }
        }

        /// <summary>
        /// Register an assembly with this typepool
        /// </summary>
        public void AddAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
        }

        /// <summary>
        /// Adds a type to this pool if it has not already been added
        /// </summary>
        public void AddType(Type type)
        {
            _types.Fill(type);
        }

        /// <summary>
        /// Adds a type to this pool if it has not already been added
        /// </summary>
        public void AddType<T>()
        {
            AddType(typeof (T));
        }

        /// <summary>
        /// Enumerates all assemblies provided either directly or
        /// through assembly sources (<see cref="AddSource"/>)
        /// </summary>
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

        /// <summary>
        /// Enumerate types that match the given filter.
        /// The type pool considers all types added directly and
        /// all exported typed from added assemblies, provided <see cref="ShouldScanAssemblies"/>
        /// is set to true
        /// </summary>
        /// <param name="filter">a filter to control type output</param>
        public IEnumerable<Type> TypesMatching(Func<Type, bool> filter)
        {
            // TODO -- diagnostics on type discovery!!!!
            return types.Where(filter).Distinct();
        }

        /// <summary>
        /// Specialize <see cref="TypesMatching"/> with regard to the type name
        /// </summary>
        public IEnumerable<Type> TypesWithFullName(string fullName)
        {
            return TypesMatching(t => t.FullName == fullName);
        }

        /// <summary>
        /// Check whether some assembly is contained in this type pool
        /// </summary>
        public bool HasAssembly(Assembly assembly)
        {
            return _assemblies.Contains(assembly);
        }

        /// <summary>
        /// Add a number of assemblies to this type pool
        /// </summary>
        public void AddAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies.AddRange(assemblies);
        }
    }
}