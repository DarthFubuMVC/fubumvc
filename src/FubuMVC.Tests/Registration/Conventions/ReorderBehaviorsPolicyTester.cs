using System;
using System.Collections.Generic;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.Nodes;
using Shouldly;
using NUnit.Framework;
using System.Linq;
using StructureMap.Pipeline;

namespace FubuMVC.Tests.Registration.Conventions
{


    [TestFixture]
    public class ReorderBehaviorsPolicyTester
    {
        private BehaviorGraph graph;
        private BehaviorChain lastChain;
        private ReorderBehaviorsPolicy policy;
        private IList<BehaviorNode> nodes;

        [SetUp]
        public void SetUp()
        {
            graph = new BehaviorGraph();
            lastChain = graph.AddChain();
            nodes = null;

            policy = new ReorderBehaviorsPolicy();
        }

        private IList<BehaviorNode> orderedNodes
        {
            get
            {
                if (nodes == null)
                {
                    policy.Configure(graph);
                    nodes = lastChain.ToList();
                }

                return nodes;
            }
        }

        private void addWrapper<T>() where T : IActionBehavior
        {
            var wrapper = new Wrapper(typeof (T));
            lastChain.AddToEnd(wrapper);
        }

        private void addNode<T>() where T : BehaviorNode, new()
        {
            var node = new T();
            lastChain.AddToEnd(node);
        }


        [Test]
        public void do_nothing_to_a_chain_if_the_order_is_correct()
        {
            addNode<Node1>();
            addNode<Node2>();
            addNode<Node3>();

            policy.ThisNodeMustBeAfter<Node3>();
            policy.ThisNodeMustBeBefore<Node1>();

            orderedNodes[0].ShouldBeOfType<Node1>();
            orderedNodes[1].ShouldBeOfType<Node2>();
            orderedNodes[2].ShouldBeOfType<Node3>();
        }

        [Test]
        public void reorder_nodes_if_the_order_is_incorrect()
        {
            addNode<Node2>();
            addNode<Node3>();
            addNode<Node1>();

            policy.ThisNodeMustBeAfter<Node2>();
            policy.ThisNodeMustBeBefore<Node1>();

            var nodes = orderedNodes;
            nodes[0].ShouldBeOfType<Node1>();
            nodes[1].ShouldBeOfType<Node2>();
            nodes[2].ShouldBeOfType<Node3>();
        }


        [Test]
        public void reorder_nodes_if_the_order_is_incorrect_2()
        {
            addNode<Node2>();
            addNode<Node3>();
            addNode<Node1>();

            policy.ThisNodeMustBeAfter<Node3>();
            policy.ThisNodeMustBeBefore<Node1>();

            orderedNodes[0].ShouldBeOfType<Node2>();
            orderedNodes[1].ShouldBeOfType<Node1>();
            orderedNodes[2].ShouldBeOfType<Node3>();
        }

        [Test]
        public void reorder_by_wrappers_positive()
        {
            addNode<Node1>();
            addNode<Node2>();
            addWrapper<Wrapper1>();

            policy.ThisNodeMustBeAfter<Node1>();
            policy.ThisWrapperBeBefore<Wrapper1>();

            orderedNodes[0].ShouldBeOfType<Wrapper>().BehaviorType.ShouldBe(typeof (Wrapper1));
            orderedNodes[1].ShouldBeOfType<Node1>();
            orderedNodes[2].ShouldBeOfType<Node2>();
        }

        [Test]
        public void reorder_by_wrappers_positive_2()
        {
            
            addNode<Node2>();
            addWrapper<Wrapper1>();
            addNode<Node1>();

            policy.ThisNodeMustBeBefore<Node1>();
            policy.ThisWrapperMustBeAfter<Wrapper1>();

            orderedNodes[0].ShouldBeOfType<Node2>();
            orderedNodes[1].ShouldBeOfType<Node1>();
            orderedNodes[2].ShouldBeOfType<Wrapper>().BehaviorType.ShouldBe(typeof(Wrapper1));
            
            
        }


        [Test]
        public void reorder_by_wrappers_negative()
        {
            addWrapper<Wrapper1>();
            addNode<Node1>();
            addNode<Node2>();
            

            policy.ThisNodeMustBeAfter<Node1>();
            policy.ThisWrapperBeBefore<Wrapper1>();

            orderedNodes[0].ShouldBeOfType<Wrapper>().BehaviorType.ShouldBe(typeof(Wrapper1));
            orderedNodes[1].ShouldBeOfType<Node1>();
            orderedNodes[2].ShouldBeOfType<Node2>();
        }


    }

    public class FakeBehavior : IActionBehavior
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

    public class Node1 : BehaviorNode
    {
        public override BehaviorCategory Category
        {
            get { throw new NotImplementedException(); }
        }

        protected override IConfiguredInstance buildInstance()
        {
            return new SmartInstance<FakeBehavior>();
        }
    }

    public class Node2 : BehaviorNode
    {
        public override BehaviorCategory Category
        {
            get { throw new NotImplementedException(); }
        }

        protected override IConfiguredInstance buildInstance()
        {
            return new SmartInstance<FakeBehavior>();
        }

    }

    public class Node3 : BehaviorNode
    {
        public override BehaviorCategory Category
        {
            get { throw new NotImplementedException(); }
        }

        protected override IConfiguredInstance buildInstance()
        {
            return new SmartInstance<FakeBehavior>();
        }

    }

    public class Wrapper1 : IActionBehavior
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

    public class Wrapper2 : IActionBehavior
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


    public class Wrapper3 : IActionBehavior
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
}