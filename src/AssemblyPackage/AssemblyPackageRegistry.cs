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
            registry.Policies.Local.Add(policy => {
                policy.Where.LastActionMatches(call => call.HandlerType == typeof (AssemblyEndpoint));
                policy.Wrap.WithBehavior<BehaviorFromAssemblyPackage>();
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