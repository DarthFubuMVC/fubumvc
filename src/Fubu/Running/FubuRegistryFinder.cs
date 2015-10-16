using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using StructureMap.Graph.Scanning;

namespace Fubu.Running
{
    public class FubuRegistryFinder : IFubuRegistryFinder
    {
        public IEnumerable<Type> Find()
        {
            var assemblies = AssemblyFinder.FindDependentAssemblies().ToArray();

            return
                TypeRepository.FindTypes(assemblies, TypeClassification.Concretes,
                    x => x.IsConcreteTypeOf<FubuRegistry>() && x.IsConcreteWithDefaultCtor()).Result();
        }
    }
}