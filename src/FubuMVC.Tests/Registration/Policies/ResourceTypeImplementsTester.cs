using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Tests.Registration.Policies
{
    [TestFixture]
    public class ResourceTypeImplementsTester
    {
        [Test]
        public void matches_positive()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<SomeEndpoints>(x => x.get_interface()));

            new ResourceTypeImplements<ISomeInterface>()
                .Matches(chain).ShouldBeTrue();
        }

        [Test]
        public void matches_negative()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<SomeEndpoints>(x => x.get_not_interface()));

            new ResourceTypeImplements<ISomeInterface>()
                .Matches(chain).ShouldBeFalse();
        }

        [Test]
        public void does_not_blow_up_if_there_is_no_resource_type()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<SomeEndpoints>(x => x.post_no_resource(null)));

            new ResourceTypeImplements<ISomeInterface>()
                .Matches(chain).ShouldBeFalse();
        }
    }

    public class SomeEndpoints
    {
        public void post_interface(ClassThatImplementsISomeInterface i)
        {
            
        }

        public void post_no_interface(ClassThatDoesNotImplementISomeInterface i)
        {
            
        }

        public void post_no_resource(ClassThatImplementsISomeInterface i)
        {
            
        }

        public ClassThatImplementsISomeInterface get_interface()
        {
            return null;
        }

        public ClassThatDoesNotImplementISomeInterface get_not_interface()
        {
            return null;
        }
    }

    public class ClassThatImplementsISomeInterface : ISomeInterface
    {
        
    }

    public class ClassThatDoesNotImplementISomeInterface
    {
        
    }

    public interface ISomeInterface
    {
    
    }
}