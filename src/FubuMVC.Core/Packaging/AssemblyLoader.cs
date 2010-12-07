using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FubuMVC.Core.Packaging
{
    public class AssemblyLoader : IAssemblyLoader, IAssemblyRegistration
    {
        public static readonly string DIRECTLY_REGISTERED_MESSAGE = "Directly loaded by the Package";

        private readonly IPackagingDiagnostics _diagnostics;
        private IPackageInfo _currentPackage;
        private readonly IList<Assembly> _assemblies = new List<Assembly>();

        public AssemblyLoader(IPackagingDiagnostics diagnostics)
        {
            AssemblyFileLoader = file =>
            {
                //return Assembly.Load(File.ReadAllBytes(file));
                return Assembly.LoadFrom(file);
            };

            _diagnostics = diagnostics;
        }

        public Func<string, Assembly> AssemblyFileLoader { get; set; }

        private bool hasAssemblyByName(string assemblyName)
        {
            // I know, packaging *ONLY* supporting one version of a dll.  Use older stuff to 
            // make redirects go
            return (_assemblies.Any(x => x.GetName().Name == assemblyName));
        }

        public void ReadPackage(IPackageInfo package)
        {
            _currentPackage = package;
            package.LoadAssemblies(this);
        }

        public IList<Assembly> Assemblies
        {
            get { return _assemblies; }
        }

        // need to try to load the assembly by name first!!!
        void IAssemblyRegistration.LoadFromFile(string fileName, string assemblyName)
        {
            if (hasAssemblyByName(assemblyName))
            {
                _diagnostics.LogDuplicateAssembly(_currentPackage, assemblyName);
            }
            else
            {
                try
                {
                    var assembly = AssemblyFileLoader(fileName);
                    _diagnostics.LogAssembly(_currentPackage, assembly, "Loaded from " + fileName);  

                    _assemblies.Add(assembly);
                }
                catch (Exception e)
                {
                    _diagnostics.LogAssemblyFailure(_currentPackage, fileName, e);
                }
            }


        }

        void IAssemblyRegistration.Use(Assembly assembly)
        {
            if (hasAssemblyByName(assembly.GetName().Name))
            {
                _diagnostics.LogDuplicateAssembly(_currentPackage, assembly.GetName().FullName);
            }
            else
            {
                _diagnostics.LogAssembly(_currentPackage, assembly, DIRECTLY_REGISTERED_MESSAGE);
                _assemblies.Add(assembly);
            }

            
        }

        public virtual void LoadAssembliesFromPackage(IPackageInfo packageInfo)
        {
            _currentPackage = packageInfo;
            packageInfo.LoadAssemblies(this);
        }
    }
    
}