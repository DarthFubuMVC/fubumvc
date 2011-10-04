using System;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Rest.Conneg;
using FubuMVC.Core.Rest.Media.Formatters;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Rest.Conneg
{
    [TestFixture]
    public class BehaviorChainConnegExtensionsTester
    {
        private BehaviorChain theChain;
        private OutputNode theOutputNode;

        [SetUp]
        public void SetUp()
        {
            theChain = BehaviorChain.For<ActionJackson>(x => x.OneInOneOut(null));
            theOutputNode = new OutputNode(typeof(OutputBehavior));

            theChain.AddToEnd(theOutputNode);
        
            // Nothing up my sleeve
            theChain.Any(x => x is ConnegNode).ShouldBeFalse();
        }

        [Test]
        public void apply_conneg_is_idempotent()
        {
            theChain.ApplyConneg();

            var inputNode = theChain.ConnegInputNode();
            var outputNode = theChain.ConnegOutputNode();

            theChain.ApplyConneg();

            theChain.ConnegInputNode().ShouldBeTheSameAs(inputNode);
            theChain.ConnegOutputNode().ShouldBeTheSameAs(outputNode);
        }

        [Test]
        public void do_not_apply_conneg_output_to_method_that_returns_HttpStatusCode()
        {
            var chain = BehaviorChain.For<ActionJackson>(x => x.CodeFor(null));
            chain.ApplyConneg();

            chain.HasConnegOutput().ShouldBeFalse();
        }

        [Test]
        public void apply_conneg_puts_the_input_node_before_any_actions()
        {
            theChain.ApplyConneg();
            theChain.First().ShouldBeOfType<ConnegInputNode>()
                .InputType.ShouldEqual(typeof (Input1));
        }

        [Test]
        public void ConnegInputNode_can_find_an_input_node_under_a_chain_if_it_exists()
        {
            theChain.ConnegInputNode().ShouldBeNull();
            theChain.ApplyConneg();
            theChain.ConnegInputNode().ShouldNotBeNull();
        }

        [Test]
        public void ConnegOutputNode_method_can_find_an_output_node_under_a_chain_if_it_exists()
        {
            theChain.ConnegOutputNode().ShouldBeNull();
            theChain.ApplyConneg();
            theChain.ConnegOutputNode().ShouldNotBeNull();
        }

        [Test]
        public void apply_conneg_puts_the_conneg_output_node_after_actions_but_before_any_other_actions()
        {
            theChain.ApplyConneg();
            theChain.Last().ShouldBeTheSameAs(theOutputNode);
            theOutputNode.Previous.ShouldBeOfType<ConnegOutputNode>()
                .InputType.ShouldEqual(typeof (Output1));
        }

        [Test]
        public void zero_in_does_not_apply_a_conneg_node()
        {
            var zeroIn = BehaviorChain.For<ActionJackson>(x => x.ZeroInOneOut());
            zeroIn.ApplyConneg();

            zeroIn.Any(x => x is ConnegInputNode).ShouldBeFalse();
            zeroIn.ConnegOutputNode().ShouldNotBeNull();
        }

        [Test]
        public void remove_conneg()
        {
            theChain.ApplyConneg();
            theChain.RemoveConneg();

            theChain.ConnegInputNode().ShouldBeNull();
            theChain.ConnegOutputNode().ShouldBeNull();
        }

        [Test]
        public void make_symmetric_json()
        {
            theChain.MakeSymmetricJson();
            var inputNode = theChain.ConnegInputNode();

            inputNode.AllowHttpFormPosts.ShouldBeFalse();
            inputNode.Readers.Any().ShouldBeFalse();
            inputNode.FormatterUsage.ShouldEqual(FormatterUsage.selected);
            inputNode.SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter));



            var outputNode = theChain.ConnegOutputNode();
            outputNode.SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter));
            outputNode.FormatterUsage.ShouldEqual(FormatterUsage.selected);
            outputNode.Writers.Any().ShouldBeFalse();
        }


        [Test]
        public void make_asymmetric_json()
        {
            theChain.MakeAsymmetricJson();
            var inputNode = theChain.ConnegInputNode();

            inputNode.AllowHttpFormPosts.ShouldBeTrue();
            inputNode.Readers.Any().ShouldBeFalse();
            inputNode.FormatterUsage.ShouldEqual(FormatterUsage.selected);
            inputNode.SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter));



            var outputNode = theChain.ConnegOutputNode();
            outputNode.SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter));
            outputNode.FormatterUsage.ShouldEqual(FormatterUsage.selected);
            outputNode.Writers.Any().ShouldBeFalse();
        }

        [Test]
        public void output_json()
        {
            theChain.ApplyConneg();
            theChain.OutputJson();

            theChain.ConnegOutputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(JsonFormatter));
        }

        [Test]
        public void output_xml()
        {
            theChain.ApplyConneg();
            theChain.OutputXml();

            theChain.ConnegOutputNode().SelectedFormatterTypes.ShouldHaveTheSameElementsAs(typeof(XmlFormatter));
        }

        
        
        public class Input1{}
        public class Output1{}
        
        public class ActionJackson
        {
            public Output1 OneInOneOut(Input1 input)
            {
                return new Output1();
            }

            public Output1 ZeroInOneOut()
            {
                return new Output1();
            }

            public HttpStatusCode CodeFor(Output1 output)
            {
                return HttpStatusCode.Unused;
            }
        }

        public class OutputBehavior : IActionBehavior
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
}