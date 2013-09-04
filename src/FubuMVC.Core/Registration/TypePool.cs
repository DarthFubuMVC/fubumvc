using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Bottles;
using FubuCore;

namespace FubuMVC.Core.Registration
{
    /// <summary>
    /// Bundle several assemblies and types to be treated as a single pool of types which can be
    /// queried and iterated over.
    /// </summary>
    public class TypePool
    {
        /// <summary>
        /// All types in the AppDomain in non dynamic assemblies
        /// </summary>
        public static TypePool AppDomainTypes()
        {
            var pool = new TypePool { IgnoreExportTypeFailures = true };
            pool.AddAssemblies(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic));

            return pool;
        }

        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly IList<Type> _types = new List<Type>();
        private bool _scanned;
        private readonly IList<Func<IEnumerable<Assembly>>> _sources = new List<Func<IEnumerable<Assembly>>>();


        /// <summary>
        /// Construct a type pool
        /// </summary>
        public TypePool()
        {
            IgnoreExportTypeFailures = true;
        }

        public bool IgnoreExportTypeFailures { get; set; }

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

                    _types.AddRange(Assemblies.Where(x => !x.IsDynamic).SelectMany(x =>
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



        /// <summary>
        ///   Finds the currently executing assembly.
        /// </summary>
        /// <returns></returns>
        public static Assembly FindTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            var thisAssembly = Assembly.GetExecutingAssembly().GetName().Name;
            var fubuCore = typeof(ITypeResolver).Assembly.GetName().Name;
            var bottles = typeof(IPackageLoader).Assembly.GetName().Name;

            Assembly callingAssembly = null;
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
                var name = assembly.GetName().Name;

                if (name != thisAssembly && name != fubuCore && name != bottles && name != "mscorlib")
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }
    }
}