using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FubuCore.Util;

namespace FubuMVC.Core.Packaging
{
    public interface IPackagingDiagnostics
    {
        void LogObject(object target, string provenance);
        void LogPackage(IPackageInfo package, IPackageLoader loader);
        void LogBootstrapperRun(IBootstrapper bootstrapper, IEnumerable<IPackageActivator> activators);
        void LogAssembly(IPackageInfo package, Assembly assembly, string provenance);
        void LogDuplicateAssembly(IPackageInfo package, string assemblyName);
        void LogAssemblyFailure(IPackageInfo package, string fileName, Exception exception);
    }

    public class PackagingDiagnostics : IPackagingDiagnostics
    {
        private readonly Cache<object, PackageRegistryLog> _logs = new Cache<object, PackageRegistryLog>(o => new PackageRegistryLog(){
            Description = o.ToString()
        });
    
        public void LogObject(object target, string provenance)
        {
            _logs[target].Provenance = provenance;
        }

        public void LogPackage(IPackageInfo package, IPackageLoader loader)
        {
            throw new NotImplementedException();
        }

        public void LogBootstrapperRun(IBootstrapper bootstrapper, IEnumerable<IPackageActivator> activators)
        {
            throw new NotImplementedException();
        }

        // TODO:  Try to find the assembly file version here. 
        public void LogAssembly(IPackageInfo package, Assembly assembly, string provenance)
        {
            throw new NotImplementedException();
        }

        public void LogDuplicateAssembly(IPackageInfo package, string assemblyName)
        {
            throw new NotImplementedException();
        }

        public void LogAssemblyFailure(IPackageInfo package, string fileName, Exception exception)
        {
            throw new NotImplementedException();
        }
    }

    public class AssemblyLoaderLog
    {
        public Assembly Assembly { get; set; }
        public IPackageInfo Package { get; set; }
        public string Provenance { get; set; }
    }

    public class PackagingRuntimeGraph
    {
        //private readonly IList<IPackageActivator> _activators = new List<IPackageActivator>();
        //private readonly IList<IPackageLoader> _loaders = new List<IPackageLoader>();
        //private readonly IList<IBootstrapper> _bootstrappers = new List<IBootstrapper>();
        private readonly Stack<string> _provenanceStack = new Stack<string>();
        


        public void PushProvenance(string provenance)
        {
            _provenanceStack.Push(provenance);
        }

        public void PopProvenance()
        {
            _provenanceStack.Pop();
        }

        //public void AddActivator(IPackageActivator activator)
        //{
        //    throw new NotImplementedException();
        //}

        public void Compile()
        {
            /*
             *  1.) Run all package loaders
             *  2.) all packages go and register assemblies with assembly loader
             *  3.) run all bootstrappers and collect the package activators
             *  4.) run each activator
             * 
             */

            throw new NotImplementedException();
        }

        public void AddLoader(IPackageLoader loader)
        {
            throw new NotImplementedException();
        }
    }

    public interface IPackageLog
    {
        void Trace(string text);
    }

    public class PackageRegistryLog : IPackageLog
    {
        private readonly StringWriter _text = new StringWriter();

        public int TimeInMilliseconds { get; set; }
        public string Provenance { get; set; }
        public string Description { get; set; }

        public void Trace(string text)
        {
            _text.WriteLine(text);
        }
    }

    public class PackageDiscovery
    {

    }
}