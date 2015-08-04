using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;

namespace AssemblyPackage
{
    public class AssemblyPackageRegistry : FubuPackageRegistry
    {
        public AssemblyPackageRegistry()
        {

        }
    }

    public class AssemblyEndpoint
    {
        public string get_hello()
        {
            return "Hello.";
        }
    }

    public class AssemblyPackageExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Local.Configure(graph =>
            {
                graph.Chains
                    .Where(x => x.HandlerTypeIs<AssemblyEndpoint>())
                    .Each(x => x.WrapWith<BehaviorFromAssemblyPackage>());
            });
        }
    }

    public class BehaviorFromAssemblyPackage : BasicBehavior
    {
        public BehaviorFromAssemblyPackage()
            : base(PartialBehavior.Executes)
        {
        }


    }
}