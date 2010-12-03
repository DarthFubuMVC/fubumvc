using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Packaging
{
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
}