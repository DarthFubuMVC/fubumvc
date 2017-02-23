using System.Security.Principal;
using System.Threading;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication
{
    
    public class ThreadPrincipalContextTester
    {
        [Fact]
        public void set_the_principal()
        {
            var principal = new GenericPrincipal(new GenericIdentity("somebody"), new string[0]);

            var context = new ThreadPrincipalContext();

            context.Current = principal;

            Thread.CurrentPrincipal.ShouldBeTheSameAs(principal);

            
        }

        [Fact]
        public void get_the_principal()
        {
            var principal = new GenericPrincipal(new GenericIdentity("somebody"), new string[0]);

            var context = new ThreadPrincipalContext();

            Thread.CurrentPrincipal = principal;

            context.Current.ShouldBeTheSameAs(principal);
        }

    }
}