using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuMVC.Core.Registration;
using StructureMap.Graph.Scanning;

namespace FubuMVC.Core.Validation
{
    public static class ValidationCompiler
    {
        public static bool IsValidationRegistration(Type type)
        {
            return type.IsConcreteWithDefaultCtor() && type.CanBeCastTo<IValidationRegistration>();
        }

        public static Task Compile(BehaviorGraph graph, IPerfTimer timer, FubuRegistry registry)
        {
            return TypeRepository.FindTypes(graph.AllAssemblies(),
                TypeClassification.Concretes | TypeClassification.Closed, IsValidationRegistration)
                .ContinueWith(t =>
                {
                        t.Result.Distinct()
                        .Each(type => registry.Services.For(typeof(IValidationRegistration)).Add(type));
                });
        }
    }
}