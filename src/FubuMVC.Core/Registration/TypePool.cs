using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FubuMVC.Core.Registration
{
    public class TypePool
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly IList<Type> _types = new List<Type>();
        private bool _scanned;
        private bool _ignoreCallingAssembly;

        public void IgnoreCallingAssembly()
        {
            _assemblies.Remove(findTheCallingAssembly());
            _ignoreCallingAssembly = true;
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

        private static Assembly findTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Assembly fubuCore = typeof (FubuCore.ITypeResolver).Assembly;

            Assembly callingAssembly = null;
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly && assembly != fubuCore)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }


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
                if (_assemblies.Any() == false && !_ignoreCallingAssembly)
                {
                    return _assemblies.Union(new Assembly[] { findTheCallingAssembly() }).Distinct();
                }
                
                return _assemblies.Distinct();
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
    }
}