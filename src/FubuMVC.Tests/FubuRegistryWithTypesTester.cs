using FubuMVC.Core;
using FubuMVC.Core.Registration.ObjectGraph;
using NUnit.Framework;
using FubuCore;
using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class FubuRegistryWithTypesTester
    {

        [Test]
        public void smoke_test_the_with_types()
        {
            var registry = new FubuRegistry();
            registry.Applies.ToThisAssembly();
            registry.WithTypes(types =>
            {
                types.TypesMatching(x => x.IsConcreteTypeOf<MyInterface>()).Each(type =>
                {
                    registry.Services(s => s.AddService(typeof(MyInterface), new ObjectDef(type)));
                });
            });

            registry.BuildGraph().Services.ServicesFor<MyInterface>()
                .Single().Type.ShouldEqual(typeof (MyConcreteClass));
        }

        public interface MyInterface{}

        public class MyConcreteClass : MyInterface
        {
            
        }
    }
}