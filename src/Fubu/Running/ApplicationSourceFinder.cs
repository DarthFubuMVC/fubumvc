using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using StructureMap;

namespace Fubu.Running
{
    public class ApplicationSourceFinder : IApplicationSourceFinder
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