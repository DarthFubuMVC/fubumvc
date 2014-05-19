using System.Linq;
using System.Reflection;
using Bottles;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap;

namespace FubuMVC.StructureMap.Testing.Internals
{
    [TestFixture]
    public class StructureMapContainerFacilityTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.For<IFileSystem>().Use<FileSystem>();
                x.For<IHttpResponse>().Use(new OwinHttpResponse());
                x.For<ICurrentChain>().Use(new CurrentChain(null, null));
                x.For<IHttpRequest>().Use(OwinHttpRequest.ForTesting());

                x.For<IResourceHash>().Use(MockRepository.GenerateMock<IResourceHash>());
            });

            container.Configure(x => x.For<IContainerFacility>().Use<StructureMapContainerFacility>());


            graph = BehaviorGraph.BuildFrom(x => {
                x.Actions.IncludeType<TestController>();

//                x.Route("/area/sub/{Name}/{Age}")
//                    .Calls<TestController>(c => c.AnotherAction(null));
//
//                x.Route("/area/sub2/{Name}/{Age}")
//                    .Calls<TestController>(c => c.AnotherAction(null));
//
//                x.Route("/area/sub3/{Name}/{Age}")
//                    .Calls<TestController>(c => c.AnotherAction(null));

                x.Models.ConvertUsing<ExampleConverter>().ConvertUsing<ExampleConverter2>();


                x.Services(s => s.AddService<IActivator>(new StubActivator()));
                x.Services(s => s.AddService<IActivator>(new StubActivator()));
                x.Services(s => s.AddService<IActivator>(new StubActivator()));
            });

            facility = new StructureMapContainerFacility(container);
            graph.As<IRegisterable>().Register(facility.Register);

            factory = facility.BuildFactory(graph);
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
        private IServiceFactory factory;
        private StructureMapContainerFacility facility;

        [Test]
        public void can_return_all_the_registered_activators_smoke_test()
        {
            facility.GetAll<IActivator>().Count().ShouldBeGreaterThan(3);
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

            myFacility.Register(typeof (ITypeResolver), new ObjectDef{
                Value = registry
            });

            myFacility.BuildFactory(new BehaviorGraph());

            myContainer.GetInstance<ITypeResolver>().ShouldBeTheSameAs(registry);
        }

        [Test]
        public void should_be_able_to_create_the_basic_services_from_the_container()
        {
            container.GetInstance<IOutputWriter>().ShouldBeOfType<OutputWriter>();
        }

        [Test]
        public void should_be_able_to_inject_multiple_implementations_as_a_dependency()
        {
            var converterFamilies =
                container.GetInstance<BindingRegistry>().AllConverterFamilies();
            converterFamilies.ShouldContain(f => f.GetType() == typeof (ExampleConverter));
            converterFamilies.ShouldContain(f => f.GetType() == typeof (ExampleConverter2));
        }

        [Test]
        public void should_be_able_to_pull_all_of_the_route_behaviors_out_of_the_container()
        {
            container.GetAllInstances<IActionBehavior>().Count().ShouldBeGreaterThan(3);
        }

        [Test]
        public void should_register_a_service_locator()
        {
            container.GetInstance<IServiceLocator>()
                .ShouldBeOfType<StructureMapServiceLocator>()
                .Container.ShouldBeTheSameAs(container);
        }

        [Test]
        public void standard_model_binder_should_not_be_registered_in_the_container()
        {
            container.GetAllInstances<IModelBinder>().Any(x => x is StandardModelBinder).ShouldBeFalse();
        }

        [Test]
        public void build_by_explicit_arguments()
        {
            var hulk = new TheHulk();
            var thor = new Thor();

            var args = new ServiceArguments().With(hulk).With(thor);
            var thing = facility.Build<ContainerThing>(args);

            thing.Hulk.ShouldBeTheSameAs(hulk);
            thing.Thor.ShouldBeTheSameAs(thor);
        }

    }

    public class ContainerThing
    {
        private readonly SpiderMan _spiderMan;
        private readonly IronMan _ironMan;
        private readonly TheHulk _hulk;
        private readonly Thor _thor;

        public ContainerThing(SpiderMan spiderMan, IronMan ironMan, TheHulk hulk, Thor thor)
        {
            _spiderMan = spiderMan;
            _ironMan = ironMan;
            _hulk = hulk;
            _thor = thor;
        }

        public SpiderMan SpiderMan
        {
            get { return _spiderMan; }
        }

        public IronMan IronMan
        {
            get { return _ironMan; }
        }

        public TheHulk Hulk
        {
            get { return _hulk; }
        }

        public Thor Thor
        {
            get { return _thor; }
        }
    }

    public class SpiderMan{}
    public class IronMan{}
    public class TheHulk{}
    public class Thor{}
}