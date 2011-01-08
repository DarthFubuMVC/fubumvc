using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using FubuMVC.StructureMap;
using FubuMVC.Tests.Packaging;
using FubuMVC.Tests.Registration;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests.StructureMapIoC
{
    

    [TestFixture]
    public class StructureMapContainerFacilityTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(x => x.For<IFileSystem>().Use<FileSystem>());
            

            graph = new FubuRegistry(x =>
            {
                x.Route<InputModel>("/area/sub/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route<InputModel>("/area/sub2/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route<InputModel>("/area/sub3/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Models.ConvertUsing<ExampleConverter>().ConvertUsing<ExampleConverter2>();
            
            
                x.Services(s => s.AddService<IActivator>(new StubActivator()));
                x.Services(s => s.AddService<IActivator>(new StubActivator()));
                x.Services(s => s.AddService<IActivator>(new StubActivator()));
            
            }).BuildGraph();

            facility = new StructureMapContainerFacility(container);
            graph.EachService(facility.Register);

            factory = facility.BuildFactory();
        }

        #endregion

        public class ExampleConverter : IConverterFamily
        {
            public bool Matches(PropertyInfo prop)
            {
                return true;
            }

            public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo prop)
            {
                return null;
            }
        }

        public class ExampleConverter2 : IConverterFamily
        {
            public bool Matches(PropertyInfo prop)
            {
                return true;
            }

            public ValueConverter Build(IValueConverterRegistry registry, PropertyInfo prop)
            {
                return null;
            }
        }

        private Container container;
        private BehaviorGraph graph;
        private IBehaviorFactory factory;
        private StructureMapContainerFacility facility;

        [Test]
        public void can_return_all_the_registered_activators_smoke_test()
        {
            facility.GetAllActivators().Count().ShouldEqual(4);
        }

        [Test]
        public void each_IActionBehavior_is_wrapped_with_a_nested_container_behavior()
        {
            graph.VisitRoutes(x =>
            {
                x.Actions += (r, chain) =>
                {
                    factory.BuildBehavior(new ServiceArguments(), chain.UniqueId).ShouldBeOfType
                        <NestedStructureMapContainerBehavior>();
                };
            });
        }

        [Test]
        public void PropertyBinderCache_should_be_a_singleton()
        {
            container.Model.For<IPropertyBinderCache>().Lifecycle.ShouldEqual("Singleton");
        }

        [Test]
        public void factory_itself_is_registered_in_the_container()
        {
            container.GetInstance<IBehaviorFactory>().ShouldBeOfType<PartialBehaviorFactory>();
        }

        [Test]
        public void factory_should_be_itself()
        {
            factory.ShouldNotBeNull();
            factory.ShouldBeTheSameAs(facility);
        }

        [Test]
        public void register_a_service_by_value()
        {
            var myContainer = new Container();
            var myFacility = new StructureMapContainerFacility(myContainer);

            var registry = new TypeResolver();

            myFacility.Register(typeof(ITypeResolver), new ObjectDef
            {
                Value = registry
            });

            myFacility.BuildFactory();

            myContainer.GetInstance<ITypeResolver>().ShouldBeTheSameAs(registry);
        }

        [Test]
        public void should_be_able_to_create_the_basic_services_from_the_container()
        {
            container.GetInstance<IOutputWriter>().ShouldBeOfType<HttpResponseOutputWriter>();
        }

        [Test]
        public void should_be_able_to_inject_multiple_implementations_as_a_dependency()
        {
            IEnumerable<IConverterFamily> converterFamilies =
                container.GetInstance<IValueConverterRegistry>().ShouldBeOfType<ValueConverterRegistry>().Families;
            converterFamilies.ShouldContain(f => f.GetType() == typeof (ExampleConverter));
            converterFamilies.ShouldContain(f => f.GetType() == typeof (ExampleConverter2));
        }

        [Test]
        public void should_be_able_to_pull_all_of_the_route_behaviors_out_of_the_container()
        {
            container.GetAllInstances<IActionBehavior>().Count.ShouldEqual(3);
        }

        [Test]
        public void should_register_a_service_locator()
        {
            container.GetInstance<IServiceLocator>()
                .ShouldBeOfType<StructureMapServiceLocator>()
                .Container.ShouldBeTheSameAs(container);
        }

        [Test]
        public void should_set_service_locator_on_fubu_page()
        {
            var serviceLocator = new StructureMapServiceLocator(container);
            container.Inject<IServiceLocator>(serviceLocator);
            var fubuPage = container.GetInstance<FubuPage>();

            fubuPage.ServiceLocator.ShouldEqual(serviceLocator);
        }

        [Test]
        public void smoke_test_get_the_current_request()
        {
            container.GetInstance<CurrentRequest>().ShouldNotBeNull();
        }

        [Test]
        public void standard_model_binder_should_not_be_registered_in_the_container()
        {
            container.GetAllInstances<IModelBinder>().Any(x => x is StandardModelBinder).ShouldBeFalse();
        }
    }
}