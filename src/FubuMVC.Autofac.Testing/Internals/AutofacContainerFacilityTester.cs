using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Autofac;

using Bottles;

using FubuCore;
using FubuCore.Binding;

using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

using FubuTestingSupport;

using NUnit.Framework;

using Rhino.Mocks;


namespace FubuMVC.Autofac.Testing.Internals
{
    [TestFixture]
    public class AutofacContainerFacilityTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<FileSystem>().As<IFileSystem>();
            builder.RegisterInstance(MockRepository.GenerateMock<IStreamingData>()).As<IStreamingData>();
            builder.RegisterInstance(new NulloHttpWriter()).As<IHttpWriter>();
            builder.RegisterInstance(new CurrentChain(null, null)).As<ICurrentChain>();
            builder.RegisterInstance(
                new StandInCurrentHttpRequest
                {
                    ApplicationRoot = "http://server"
                }).As<ICurrentHttpRequest>();
            builder.RegisterInstance(MockRepository.GenerateMock<IResourceHash>()).As<IResourceHash>();
            builder.RegisterType<AutofacContainerFacility>().As<IContainerFacility>();
            context = builder.Build();

            graph = BehaviorGraph.BuildFrom(
                x =>
                {
                    x.Route("/area/sub/{Name}/{Age}")
                     .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                    x.Route("/area/sub2/{Name}/{Age}")
                     .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                    x.Route("/area/sub3/{Name}/{Age}")
                     .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                    x.Models.ConvertUsing<ExampleConverter>().ConvertUsing<ExampleConverter2>();


                    x.Services(s => s.AddService<IActivator>(new StubActivator()));
                    x.Services(s => s.AddService<IActivator>(new StubActivator()));
                    x.Services(s => s.AddService<IActivator>(new StubActivator()));
                });

            facility = new AutofacContainerFacility(context);
            graph.As<IRegisterable>().Register(facility.Register);

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


        private IComponentContext context;
        private BehaviorGraph graph;
        private IServiceFactory factory;
        private AutofacContainerFacility facility;

        [Test]
        public void can_return_all_the_registered_activators_smoke_test()
        {
            facility.GetAll<IActivator>().Count().ShouldEqual(3);
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
            var builder = new ContainerBuilder();
            var myContext = builder.Build();
            var myFacility = new AutofacContainerFacility(myContext);

            var registry = new TypeResolver();

            myFacility.Register(
                typeof(ITypeResolver),
                new ObjectDef
                {
                    Value = registry
                });

            myFacility.BuildFactory();

            myContext.Resolve<ITypeResolver>().ShouldBeTheSameAs(registry);
        }

        [Test]
        public void should_be_able_to_create_the_basic_services_from_the_container()
        {
            context.Resolve<IOutputWriter>().ShouldBeOfType<OutputWriter>();
        }

        [Test]
        public void should_be_able_to_inject_multiple_implementations_as_a_dependency()
        {
            var converterFamilies = context.Resolve<BindingRegistry>().AllConverterFamilies().ToList();
            converterFamilies.ShouldContain(f => f.GetType() == typeof(ExampleConverter));
            converterFamilies.ShouldContain(f => f.GetType() == typeof(ExampleConverter2));
        }

        [Test]
        public void should_be_able_to_pull_all_of_the_route_behaviors_out_of_the_container()
        {
            context.Resolve<IEnumerable<IActionBehavior>>().Count().ShouldEqual(3);
        }

        [Test]
        public void should_register_a_service_locator()
        {
            context.Resolve<IServiceLocator>()
                   .ShouldBeOfType<AutofacServiceLocator>();
        }

        [Test]
        public void standard_model_binder_should_not_be_registered_in_the_container()
        {
            context.Resolve<IEnumerable<IModelBinder>>().Any(x => x is StandardModelBinder).ShouldBeFalse();
        }
    }
}