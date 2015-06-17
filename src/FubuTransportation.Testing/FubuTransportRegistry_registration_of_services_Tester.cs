using FubuMVC.Core.Registration;
using FubuTransportation.Configuration;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class FubuTransportRegistry_registration_of_services_Tester
    {
        [Test]
        public void build_services_for_a_service_registry()
        {
            var graph = FubuTransportRegistry.BehaviorGraphFor(x => x.Services<FooServiceRegistry>());
            graph.Services.ServicesFor<IFoo>().Select(x => x.Type)
                .ShouldHaveTheSameElementsAs(typeof(GreenFoo), typeof(RedFoo));
            
        }

        [Test]
        public void build_services_inline()
        {
            var graph = FubuTransportRegistry.BehaviorGraphFor(x => {
                x.Services(s => {
                    s.AddService<IFoo, BlueFoo>();
                    s.AddService<IFoo, RedFoo>();
                });
            });

            graph.Services.ServicesFor<IFoo>().Select(x => x.Type)
                .ShouldHaveTheSameElementsAs(typeof(BlueFoo), typeof(RedFoo));
        }
    }

    public class FooRegistration : FubuTransportRegistry
    {
        public FooRegistration()
        {
            Services<FooServiceRegistry>();
        }
    }

    public class FooServiceRegistry : ServiceRegistry
    {
        public FooServiceRegistry()
        {
            AddService<IFoo, GreenFoo>();
            AddService<IFoo, RedFoo>();
        }
    }

    public interface IFoo
    {
        
    }



    public class RedFoo : IFoo{}
    public class GreenFoo : IFoo{}
    public class BlueFoo : IFoo{}
}