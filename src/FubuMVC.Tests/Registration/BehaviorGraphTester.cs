using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Routes;
using NUnit.Framework;
using Shouldly;

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

        public class MyOtherRequestModel
        {
        }

        public class MyRequestModel
        {
        }

        [Test]
        public void requires_session_state_is_true_by_default()
        {
            var graph = new BehaviorGraph();

            graph.Settings.Get<SessionStateRequirement>().ShouldBe(SessionStateRequirement.RequiresSessionState);
        }


        [Test]
        public void find_home_is_not_set()
        {
            var graph = BehaviorGraph.BuildFrom(x =>
            {
                x.Actions.DisableDefaultActionSource();
                x.Actions.IncludeClassesSuffixedWithController();
            });

            graph.FindHomeChain().ShouldBeNull();
        }

        [Test]
        public void should_remove_chain()
        {
            var graph = BehaviorGraph.BuildFrom(x => { x.Actions.IncludeClassesSuffixedWithController(); });

            var chain = graph.ChainFor<MyHomeController>(x => x.ThisIsHome());
            graph.RemoveChain(chain);

            graph
                .Chains
                .ShouldNotContain(chain);
        }

        [Test]
        public void explicit_version()
        {
            var graph = new BehaviorGraph();
            graph.Version = "2.3";

            graph.Version.ShouldBe("2.3");
        }

        [Test]
        public void derive_if_the_assembly_is_set()
        {
            var graph = new BehaviorGraph
            {
                ApplicationAssembly = Assembly.GetExecutingAssembly()
            };


            graph.Version.ShouldBe(Assembly.GetExecutingAssembly().GetName().Version.ToString());
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