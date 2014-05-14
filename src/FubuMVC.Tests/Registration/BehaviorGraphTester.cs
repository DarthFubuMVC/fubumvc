using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

            });

            var chain = graph.BehaviorFor<MyHomeController>(x => x.ThisIsHome());
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

        [Test]
        public void explicit_version()
        {
            var graph = new BehaviorGraph();
            graph.Version = "2.3";

            graph.Version.ShouldEqual("2.3");
        }

        [Test]
        public void derive_if_the_assembly_is_set()
        {
            var graph = new BehaviorGraph()
            {
                ApplicationAssembly = Assembly.GetExecutingAssembly()
            };



            graph.Version.ShouldEqual(Assembly.GetExecutingAssembly().GetName().Version);
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

            graph1.As<IChainImporter>().Import(graph2.Behaviors);
        }

        #endregion

        private BehaviorGraph graph1;
        private BehaviorGraph graph2;
        private Foo foo1;
        private Foo foo2;
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