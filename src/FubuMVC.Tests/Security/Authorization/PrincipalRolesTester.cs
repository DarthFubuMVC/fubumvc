using System.Security.Principal;
using System.Threading;
using FubuMVC.Core.Security.Authorization;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authorization
{
    
    public class PrincipalRolesTester
    {
        [Fact]
        public void read_role_off_principal()
        {
            var principal = new GenericPrincipal(new GenericIdentity("somebody"), new string[]{"a", "b"});
            Thread.CurrentPrincipal = principal;

            PrincipalRoles.Current.ShouldBeTheSameAs(principal);

            PrincipalRoles.IsInRole("a").ShouldBeTrue();
            PrincipalRoles.IsInRole("c").ShouldBeFalse();
        }
    }
}