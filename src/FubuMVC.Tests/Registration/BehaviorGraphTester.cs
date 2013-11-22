using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using TestPackage1.FakeControllers;

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

            public string get_home()
            {
                return "hello";
            }

            public string get_other_home(MyOtherRequestModel model)
            {
                return "hello";
            }
        }

        public class MyOtherRequestModel{}

        public class MyRequestModel
        {
        }

        [Test]
        public void requires_session_state_is_true_by_default()
        {
            var graph = new BehaviorGraph();

            graph.Settings.Get<SessionStateRequirement>().ShouldEqual(SessionStateRequirement.RequiresSessionState);
        }

        [Test]
        public void RegisterService_can_be_called_multiple_times_to_store_multiple_implementations()
        {
            var graph = new BehaviorGraph();
            graph.Services.AddService<IRequestData, RequestData>();
            graph.Services.AddService<IRequestData, InMemoryRequestData>();

            var implementations = new List<Type>();
            graph.As<IRegisterable>().Register(
                (t, def) => { if (t == typeof (IRequestData)) implementations.Add(def.Type); });

            implementations.ShouldContain(typeof (RequestData));
            implementations.ShouldContain(typeof (InMemoryRequestData));
        }

        [Test]
        public void description_writes_each_behavior_first_call_and_route_pattern()
        {
            var graph = BehaviorGraph.BuildFrom(x => {
                x.Actions.IncludeType<OneController>();

                x.Policies.Add(policy => {
                    policy.Wrap.WithBehavior<WrappingBehavior2>();
                    policy.Wrap.WithBehavior<WrappingBehavior>();
                });

            });

            var listener = MockRepository.GenerateStub<TraceListener>();
            Trace.Listeners.Add(listener);
            graph.Describe();
            graph.Behaviors.Each(
                b => listener.AssertWasCalled(
                    l => l.WriteLine(b.FirstCall().Description.PadRight(70) + b.Route.Pattern)));
        }

        [Test]
        public void find_home_is_not_null()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeClassesSuffixedWithController();

                x.Routes.HomeIs<MyHomeController>(c => c.ThisIsHome());
            });

            graph.FindHomeChain().FirstCall().Method.Name.ShouldEqual("ThisIsHome");
        }

        [Test]
        public void home_url()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeClassesSuffixedWithController();

                x.Routes.HomeIs<MyHomeController>(c => c.ThisIsHome());
            });

            graph.FindHomeChain().GetRoutePattern().ShouldEqual("");
        }

        [Test]
        public void home_url_keeps_the_http_constraints()
        {
            var graph = BehaviorGraph.BuildFrom(x => {
                x.Actions.IncludeType<MyHomeController>();

                x.Routes.HomeIs<MyHomeController>(c => c.get_home());
            });

            graph.FindHomeChain().Route.AllowedHttpMethods.Single()
                .ShouldEqual("GET");
        }

        [Test]
        public void home_url_keeps_the_http_constraints_by_input_model()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeType<MyHomeController>();

                x.Routes.HomeIs<MyOtherRequestModel>();
            });

            graph.FindHomeChain().Route.AllowedHttpMethods.Single()
                .ShouldEqual("GET");
        }

        [Test]
        public void find_home_is_not_set()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeClassesSuffixedWithController();
            });

            graph.FindHomeChain().ShouldBeNull();
        }

        [Test]
        public void should_remove_chain()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.IncludeClassesSuffixedWithController();

                x.Routes.HomeIs<MyHomeController>(c => c.ThisIsHome());
            });

            var chain = graph.FindHomeChain();
            graph.RemoveChain(chain);

            graph
                .Behaviors
                .ShouldNotContain(chain);
        }

        [Test]
        public void the_first_call_to_RegisterService_for_a_type_registers_the_default()
        {
            var graph = new BehaviorGraph();
            graph.Services.AddService<IRequestData, RequestData>();
            graph.Services.DefaultServiceFor<IRequestData>().Type.ShouldEqual(typeof (RequestData));
        }
    }

    public class WrappingBehavior : IActionBehavior
    {
        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }
    }

    public class WrappingBehavior2 : IActionBehavior
    {
        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class when_finding_the_id_for_a_call
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new BehaviorGraph();

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
            Exception<FubuException>.ShouldBeThrownBy(() => { new BehaviorGraph().IdForType(typeof (Model1)); }).
                ErrorCode.ShouldEqual(2150);
        }

        [Test]
        public void should_throw_2151_if_the_input_type_has_multiple_possible_behavior_chains()
        {
            var graph = new BehaviorGraph();

            graph.AddChain(BehaviorChain.For<ControllerTarget>(x => x.OneInOneOut(null)));
            graph.AddChain(BehaviorChain.For<ControllerTarget>(x => x.OneInZeroOut(null)));

            Exception<FubuException>.ShouldBeThrownBy(() => { graph.IdForType(typeof (Model1)); }).ErrorCode.ShouldEqual
                (2151);
        }

        [Test]
        public void when_there_is_only_one_chain_for_that_input_model()
        {
            var graph = new BehaviorGraph();

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
            graph1 = BehaviorGraph.BuildFrom(x =>
            {
                x.Route("method1/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route("method2/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();

                x.Route("method3/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();
            });

            chain = new BehaviorChain();
            graph1.AddChain(chain);

            graph2 = BehaviorGraph.BuildFrom(x =>
            {
                x.Route("/root/{Name}/{Age}")
                    .Calls<TestController>(c => c.AnotherAction(null)).OutputToJson();
            });

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
            graph1 = new BehaviorGraph();
            graph2 = new BehaviorGraph();

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