using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Security.Authentication;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Security.Authentication
{
    
    public class AuthenticationFilterNodeTester
    {
        private AuthenticationFilterNode theNode = new AuthenticationFilterNode();


        [Fact]
        public void has_to_be_authentication()
        {
            theNode.Category.ShouldBe(BehaviorCategory.Authentication);
        }

        public class SomeDependency { }
    }
}