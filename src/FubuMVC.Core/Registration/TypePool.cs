using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FubuMVC.Core.Registration
{
    public class TypePool
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly IList<Type> _types = new List<Type>();
        private bool _scanned;

        private IList<Type> types
        {
            get
            {
                if (!_scanned)
                {
                    _scanned = true;

                    // TODO:  Good exception message when an assembly blows up on 
                    // GetExportedTypes()
                    _types.AddRange(_assemblies.SelectMany(x => x.GetExportedTypes()));
                }


                return _types;
            }
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


        public IEnumerable<Type> TypesMatching(Func<Type, bool> filter)
        {
            return types.Where(filter);
        }
    }
}