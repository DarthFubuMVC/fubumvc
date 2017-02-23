using System.Linq;
using Shouldly;
using Xunit;

namespace FubuMVC.IntegrationTesting.Packaging
{
    
    public class assembly_marked_as_FubuModule_is_added_as_a_package 
    {
        [Fact]
        public void has_endpoints_from_AssemblyPackage()
        {
            TestHost.Runtime.Behaviors.Chains.Where(x => x.Calls.Any(_ => _.HandlerType.Assembly.GetName().Name == "AssemblyPackage"))
                .Any().ShouldBeTrue();
        }
    }
}