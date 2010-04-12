using System;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using System.Collections.Generic;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class attaching_behaviors_to_action_calls
    {
        private BehaviorGraph graph;

        [SetUp]
        public void SetUp()
        {
            graph = new FubuRegistry(x =>
            {
                x.Applies.ToThisAssembly();
                x.Actions.IncludeTypesNamed(o => o.EndsWith("Controller"));
            }).BuildGraph();
        }

        [Test]
        public void can_prepend_behaviors_in_front_of_an_action()
        {
            graph.Actions().Each(x => x.AddBefore(Wrapper.For<MyWrapper>()));

            graph.Actions().Each(x =>
            {
                x.Previous.ShouldBeOfType<Wrapper>();
            });
        }


        [Test]
        public void can_prepend_behaviors_in_front_of_an_action_2()
        {
            graph.Actions().Each(x => x.WrapWith<MyWrapper>());

            graph.Actions().Each(x =>
            {
                x.Previous.ShouldBeOfType<Wrapper>();
            });
        }


        [Test]
        public void can_prepend_behaviors_in_front_of_an_action_3()
        {
            graph.Actions().Each(x => x.WrapWith(typeof(MyWrapper)));

            graph.Actions().Each(x =>
            {
                x.Previous.ShouldBeOfType<Wrapper>();
            });
        }
    }

    public class MyWrapper : BasicBehavior
    {
        public MyWrapper()
            : base(PartialBehavior.Executes)
        {
        }

        protected override DoNext performInvoke()
        {
            throw new NotImplementedException();
        }
    }
}