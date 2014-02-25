using System;
using System.Linq;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime;
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
        }

        #endregion

        private BehaviorChain theChain;

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
        public void make_asymmetric_json()
        {
            theChain.MakeAsymmetricJson();
            var inputNode = theChain.Input;

            inputNode.CanRead(MimeType.HttpFormMimetype).ShouldBeTrue();
            inputNode.Readers.ShouldHaveCount(2);
            inputNode.CanRead(MimeType.Json).ShouldBeTrue();
            

            var outputNode = theChain.Output;
            outputNode.Media().ShouldHaveCount(1);
            outputNode.Writes(MimeType.Json).ShouldBeTrue();
        }

        [Test]
        public void make_symmetric_json()
        {
            theChain.MakeSymmetricJson();
            var inputNode = theChain.Input;

            inputNode.CanRead(MimeType.HttpFormMimetype).ShouldBeFalse();
            inputNode.Readers.ShouldHaveCount(1);
            inputNode.CanRead(MimeType.Json).ShouldBeTrue();


            var outputNode = theChain.Output;
            outputNode.Media().ShouldHaveCount(1);
            outputNode.Writes(MimeType.Json).ShouldBeTrue();
        }

        [Test]
        public void output_json()
        {
            theChain.OutputJson();

            var outputNode = theChain.Output;
            outputNode.Media().ShouldHaveCount(1);
            outputNode.Writes(MimeType.Json).ShouldBeTrue();
        }

        [Test]
        public void output_xml()
        {
            theChain.OutputXml();

            var outputNode = theChain.Output;
            outputNode.Media().ShouldHaveCount(1);
            outputNode.Writes(MimeType.Xml).ShouldBeTrue();
        }

        [Test]
        public void remove_conneg()
        {
            theChain.RemoveConneg();

            theChain.HasReaders().ShouldBeFalse();
            theChain.Output.Media().Any().ShouldBeFalse();
        }
    }
}