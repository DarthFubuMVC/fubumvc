using System;
using System.Linq;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Formatters;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Resources.Conneg
{
    [TestFixture]
    public class BehaviorChainConnegExtensionsTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            theChain = BehaviorChain.For<ActionJackson>(x => x.OneInOneOut(null));
            theOutputNode = new OutputNode(typeof (OutputBehavior));

            theChain.AddToEnd(theOutputNode);
        }

        #endregion

        private BehaviorChain theChain;
        private OutputNode theOutputNode;

        public class Input1
        {
        }

        public class Output1
        {
        }

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

        [Test]
        public void ConnegInputNode_can_find_an_input_node_under_a_chain_if_it_exists()
        {
            theChain.Input.ShouldBeNull();
            theChain.ApplyConneg();
            theChain.Input.ShouldNotBeNull();
        }

        [Test]
        public void ConnegOutputNode_method_can_find_an_output_node_under_a_chain_if_it_exists()
        {
            theChain.Output.ShouldBeNull();
            theChain.ApplyConneg();
            theChain.Output.ShouldNotBeNull();
        }

        [Test]
        public void apply_conneg_is_idempotent()
        {
            theChain.ApplyConneg();

            var inputNode = theChain.Input;
            var outputNode = theChain.Output;

            theChain.ApplyConneg();

            theChain.Input.ShouldBeTheSameAs(inputNode);
            theChain.Output.ShouldBeTheSameAs(outputNode);
        }

        [Test]
        public void do_not_apply_conneg_output_to_method_that_returns_HttpStatusCode()
        {
            var chain = BehaviorChain.For<ActionJackson>(x => x.CodeFor(null));
            chain.ApplyConneg();

            chain.HasConnegOutput().ShouldBeFalse();
        }


        [Test]
        public void make_asymmetric_json()
        {
            theChain.MakeAsymmetricJson();
            var inputNode = theChain.Input;

            inputNode.AllowHttpFormPosts.ShouldBeTrue();
            inputNode.Readers.ShouldHaveCount(2);
            inputNode.UsesFormatter<JsonFormatter>();


            var outputNode = theChain.Output;
            outputNode.Writers.ShouldHaveCount(1);
            outputNode.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void make_symmetric_json()
        {
            theChain.MakeSymmetricJson();
            var inputNode = theChain.Input;

            inputNode.AllowHttpFormPosts.ShouldBeFalse();
            inputNode.Readers.ShouldHaveCount(1);
            inputNode.UsesFormatter<JsonFormatter>();


            var outputNode = theChain.Output;
            outputNode.Writers.ShouldHaveCount(1);
            outputNode.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void output_json()
        {
            theChain.ApplyConneg();
            theChain.OutputJson();

            var outputNode = theChain.Output;
            outputNode.Writers.ShouldHaveCount(1);
            outputNode.UsesFormatter<JsonFormatter>().ShouldBeTrue();
        }

        [Test]
        public void output_xml()
        {
            theChain.ApplyConneg();
            theChain.OutputXml();

            var outputNode = theChain.Output;
            outputNode.Writers.ShouldHaveCount(1);
            outputNode.UsesFormatter<XmlFormatter>().ShouldBeTrue();
        }

        [Test]
        public void remove_conneg()
        {
            theChain.ApplyConneg();
            theChain.RemoveConneg();

            theChain.Input.ShouldBeNull();
            theChain.Output.ShouldBeNull();
        }

        [Test]
        public void zero_in_does_not_apply_a_conneg_node()
        {
            var zeroIn = BehaviorChain.For<ActionJackson>(x => x.ZeroInOneOut());
            zeroIn.ApplyConneg();

            zeroIn.HasReaders().ShouldBeFalse();
            zeroIn.Output.ShouldNotBeNull();
        }
    }
}