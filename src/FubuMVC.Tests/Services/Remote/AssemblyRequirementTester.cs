using FubuMVC.Core.Services.Remote;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Services.Remote
{
    
    public class AssemblyRequirementTester
    {
        [Fact]
        public void is_sem_ver_compatible()
        {
            AssemblyRequirement.IsSemVerCompatible("1.0", "1.1").ShouldBeTrue();
            AssemblyRequirement.IsSemVerCompatible("1.0", "1.2").ShouldBeTrue();
            AssemblyRequirement.IsSemVerCompatible("1.0", "1.3").ShouldBeTrue();
            AssemblyRequirement.IsSemVerCompatible("1.1", "1.3").ShouldBeTrue();
            AssemblyRequirement.IsSemVerCompatible("1.2", "1.3").ShouldBeTrue();
            AssemblyRequirement.IsSemVerCompatible("1.3", "1.3").ShouldBeTrue();
            AssemblyRequirement.IsSemVerCompatible("1.0", "1.4").ShouldBeTrue();
            AssemblyRequirement.IsSemVerCompatible("1.0", "2.0").ShouldBeFalse();
            AssemblyRequirement.IsSemVerCompatible("1.1", "2.0").ShouldBeFalse();
            AssemblyRequirement.IsSemVerCompatible("1.5", "2.0").ShouldBeFalse();
            AssemblyRequirement.IsSemVerCompatible("1.5", "1.4").ShouldBeFalse();
        }
    }
}