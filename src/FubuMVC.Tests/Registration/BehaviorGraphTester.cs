using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Diagnostics.Tracing;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Runtime;
using FubuMVC.FakeControllers;
using FubuMVC.Tests.Diagnostics;
using FubuMVC.WebForms;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class BehaviorGraphTester
    {
        public class FakeController
        {
            public void Query(FakeInputModel model)
            {
            }
        }

        public class FakeInputModel
        {
        }

        [Test]
        public void RegisterService_can_be_called_multiple_times_to_store_multiple_implementations()
        {
            var graph = new BehaviorGraph(null);
            graph.Services.AddService<IRequestData, RequestData>();
            graph.Services.AddService<IRequestData, InMemoryRequestData>();

            var implementations = new List<Type>();
            graph.As<IRegisterable>().Register(DiagnosticLevel.None, (t, def) => { if (t == typeof (IRequestData)) implementations.Add(def.Type); });

            implementations.ShouldContain(typeof (RequestData));
            implementations.ShouldContain(typeof (InMemoryRequestData));
        }

        [Test]
        public void description_writes_each_behavior_first_call_and_route_pattern()
        {
            var graph = new FubuRegistry(x =>
            {
                x.Applies.ToAssemblyContainingType<OneController>();
                x.Policies.WrapBehaviorChainsWith<WrappingBehavior2>();
                x.Policies.WrapBehaviorChainsWith<WrappingBehavior>();
            }).BuildGraph();

            var listener = MockRepository.GenerateStub<TraceListener>();
            Trace.Listeners.Add(listener);
            graph.Describe();
            graph.Behaviors.Each(
                b => listener.AssertWasCalled(
                    l => l.WriteLine(b.FirstCall().Description.PadRight(70) + b.Route.Pattern)));
        }

        [Test]
        public void find_home_is_not_set()
        {
            var graph = new FubuRegistry(x =>
            {
                x.Applies.ToThisAssembly();
                x.Actions.IncludeClassesSuffixedWithController();
            }).BuildGraph();

            graph.FindHomeChain().ShouldBeNull();
        }

        [Test]
        public void find_home_is_not_null()
        {
            var graph = new FubuRegistry(x =>
            {
                x.Applies.ToThisAssembly();
                x.Actions.IncludeClassesSuffixedWithController();

                x.Routes.HomeIs<MyHomeController>(c => c.ThisIsHome());
            }).BuildGraph();

            graph.FindHomeChain().FirstCall().Method.Name.ShouldEqual("ThisIsHome");
        }

        public class MyHomeController
        {
            public string ThisIsHome()
            {
                return string.Empty;
            }

            public string ThisIsAnotherAction(MyRequestModel request)
            {
                return string.Empty;
            }
        }

        public class MyRequestModel {}

        [Test]
        public void replace_service_by_specifying_a_value()
        {
            var graph = new BehaviorGraph(null);
            Assert.Fail("Use something else here");
            //var resolver = new RecordingObjectResolver(null, null);

            //graph.Services.ReplaceService<IObjectResolver>(resolver);

            //graph.Services.DefaultServiceFor<IObjectResolver>().Value.ShouldBeTheSameAs(resolver);
        }

        [Test]
        public void replace_service_by_specifying_types()
        {
            var graph = new BehaviorGraph(null);
            graph.Services.ReplaceService<IOutputWriter, RecordingOutputWriter>();

            graph.Services.DefaultServiceFor<IOutputWriter>().Type.ShouldEqual(typeof (RecordingOutputWriter));
        }

        [Test]
        public void the_first_call_to_RegisterService_for_a_type_registers_the_default()
        {
            var graph = new BehaviorGraph(null);
            graph.Services.AddService<IRequestData, RequestData>();
            graph.Services.DefaultServiceFor<IRequestData>().Type.ShouldEqual(typeof (RequestData));
        }

        [Test]
        public void should_remove_chain()
        {
            var graph = new FubuRegistry(x =>
                                             {
                                                 x.Applies.ToThisAssembly();
                                                 x.Actions.IncludeClassesSuffixedWithController();

                                                 x.Routes.HomeIs<MyHomeController>(c => c.ThisIsHome());
                                             }).BuildGraph();
            
            var chain = graph.FindHomeChain();
            graph.RemoveChain(chain);

            graph
                .Behaviors
                .ShouldNotContain(chain);
        }

    }


    [TestFixture]
    public class when_finding_the_id_for_a_call
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new BehaviorGraph(null);

            chain1 = BehaviorChain.For<ControllerTarget>(x => x.OneInOneOut(null));
            graph.AddChain(chain1);

            chain2 = BehaviorChain.For<ControllerTarget>(x => x.OneInZeroOut(null));
            graph.AddChain(chain2);
        }

        #endregion

        private BehaviorGraph graph;
        private BehaviorChain chain1;
        private BehaviorChain chain2;

        [Test]
        public void find_the_chain_matching_the_call()
        {
            graph.IdForCall(ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null))).ShouldEqual(chain1.UniqueId);
            graph.IdForCall(ActionCall.For<ControllerTarget>(x => x.OneInZeroOut(null))).ShouldEqual(chain2.UniqueId);
        }

        [Test]
        public void throw_2152_when_the_call_cannot_be_found()
        {
            Exception<FubuException>.ShouldBeThrownBy(
                () => { graph.IdForCall(ActionCall.For<ControllerTarget>(x => x.ZeroInOneOut())); }).ErrorCode.
                ShouldEqual(2152);
        }
    }


    [TestFixture]
    public class when_finding_the_id_of_an_input_model
    {
        [Test]
        public void should_throw_2150_if_the_input_type_cannot_be_found()
        {
            Exception<FubuException>.ShouldBeThrownBy(() => { new BehaviorGraph(null).IdForType(typeof (Model1)); }).
                ErrorCode.ShouldEqual(2150);
        }

        [Test]
        public void should_throw_2151_if_the_input_type_has_multiple_possible_behavior_chains()
        {
            var graph = new BehaviorGraph(null);

            graph.AddChain(BehaviorChain.For<ControllerTarget>(x => x.OneInOneOut(null)));
            graph.AddChain(BehaviorChain.For<ControllerTarget>(x => x.OneInZeroOut(null)));

            Exception<FubuException>.ShouldBeThrownBy(() => { graph.IdForType(typeof (Model1)); }).ErrorCode.ShouldEqual
                (2151);
        }

        [Test]
        public void when_there_is_only_one_chain_for_that_input_model()
        {
            var graph = new BehaviorGraph(null);

            var chain = BehaviorChain.For<ControllerTarget>(x => x.OneInOneOut(null));
            graph.AddChain(chain);
            graph.IdForType(typeof (Model1)).ShouldEqual(chain.UniqueId);
        }
    }

    [TestFixture]
    public class when_importing_urls
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph1 = new FubuRegistry(x =>
            {
                x.Route("method1/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route("method2/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route("method3/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();
            }).BuildGraph();

            chain = new BehaviorChain();
            graph1.AddChain(chain);

            graph2 = new FubuRegistry(x =>
            {
                x.Route("/root/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();
            }).BuildGraph();

            graph2.As<IChainImporter>().Import(graph1, b => b.PrependToUrl("area1"));
        }

        #endregion

        private BehaviorGraph graph1;
        private BehaviorGraph graph2;
        private BehaviorChain chain;

        [Test]
        public void should_have_all_the_routes_from_the_imported_graph()
        {
            graph2.Routes.Any(x => x.Pattern == "area1/method1/{Name}/{Age}").ShouldBeTrue();
            graph2.Routes.Any(x => x.Pattern == "area1/method2/{Name}/{Age}").ShouldBeTrue();
            graph2.Routes.Any(x => x.Pattern == "area1/method3/{Name}/{Age}").ShouldBeTrue();
        }

        [Test]
        public void should_have_imported_the_behavior_chains_without_routes()
        {
            graph2.Behaviors.Contains(chain).ShouldBeTrue();
        }
    }


    [TestFixture]
    public class when_merging_services
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph1 = new BehaviorGraph(null);
            graph2 = new BehaviorGraph(null);

            foo1 = new Foo();
            foo2 = new Foo();

            graph1.Services.AddService(foo1);
            graph2.Services.AddService(foo2);

            graph1.As<IChainImporter>().Import(graph2, b => b.PrependToUrl(string.Empty));
        }

        #endregion

        private BehaviorGraph graph1;
        private BehaviorGraph graph2;
        private Foo foo1;
        private Foo foo2;
    }

    [TestFixture]
    public class when_registering_partial_view_types
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _graph = new FubuRegistry(registry =>
            {
                registry.RegisterPartials(x => x.For<PartialModel>().Use<PartialView>());
                registry.RegisterPartials(x =>
                {
                    x.For<PartialModel1>().Use<PartialView1>();
                    x.For<PartialModel2>().Use<PartialView2>();
                });
            }).BuildGraph();

            _partialViewTypeRegistries = _graph.Services.FindAllValues<IPartialViewTypeRegistry>();
            _partialViewTypeRegistries.ShouldHaveCount(1);
        }

        #endregion

        private BehaviorGraph _graph;
        private IEnumerable<IPartialViewTypeRegistry> _partialViewTypeRegistries;

        public class PartialView : FubuPage
        {
        }

        public class PartialModel
        {
        }

        public class PartialView1 : FubuPage
        {
        }

        public class PartialModel1
        {
        }

        public class PartialView2 : FubuPage
        {
        }

        public class PartialModel2
        {
        }

        public class UnregisteredPartialModel
        {
        }

        public class UnregisteredPartialView : FubuPage
        {
        }

        [Test]
        public void registry_should_contain_registered_types()
        {
            _partialViewTypeRegistries.ShouldHave(reg => reg.HasPartialViewTypeFor<PartialModel>() &&
                                                         reg.HasPartialViewTypeFor<PartialModel1>() &&
                                                         reg.HasPartialViewTypeFor<PartialModel2>());
        }

        [Test]
        public void registry_should_not_contain_unregistered_type()
        {
            _partialViewTypeRegistries.ShouldNotHave(reg => reg.HasPartialViewTypeFor<UnregisteredPartialModel>());
        }
    }

    [TestFixture]
    public class when_adding_chains_by_action
    {
        [Test]
        public void add_a_simple_closed_type()
        {
            var graph = new BehaviorGraph();
            var chain = graph.AddActionFor("go/{Id}", typeof (Action1));

            chain.FirstCall().HandlerType.ShouldEqual(typeof (Action1));
            chain.FirstCall().Method.Name.ShouldEqual("Go");
            chain.Route.Pattern.ShouldEqual("go/{Id}");
            chain.Route.Input.ShouldBeOfType<RouteInput<ArgModel>>();

            chain.Route.CreateUrlFromInput(new ArgModel{
                Id = 5
            }).ShouldEqual("go/5");

            graph.Behaviors.Count().ShouldEqual(1);
        }

        [Test]
        public void add_a_simple_open_type()
        {
            var graph = new BehaviorGraph();
            var chain = graph.AddActionFor("go/{Id}", typeof (Action2<>), typeof (string));

            chain.FirstCall().HandlerType.ShouldEqual(typeof (Action2<string>));
            chain.FirstCall().Method.Name.ShouldEqual("Go");
            chain.Route.Pattern.ShouldEqual("go/{Id}");
            chain.Route.Input.ShouldBeOfType<RouteInput<ArgModel>>();
            chain.Route.CreateUrlFromInput(new ArgModel{
                Id = 5
            }).ShouldEqual("go/5");

            graph.Behaviors.Count().ShouldEqual(1);
        }
    }

    public class Action1
    {
        public void Go(ArgModel model)
        {
        }
    }

    public class Action2<T>
    {
        public void Go(ArgModel model)
        {
        }
    }

    public class ArgModel
    {
        public long Id { get; set; }
    }

    public class Foo
    {
    }
}