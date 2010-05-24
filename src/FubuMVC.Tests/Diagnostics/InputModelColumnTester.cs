using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Tests.Registration;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class InputModelColumnTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void write_body_for_chain_with_input_type()
        {
            var chain = new BehaviorChain();
            chain.AddToEnd(ActionCall.For<ControllerTarget>(x => x.OneInOneOut(null)));
            var tag = new HtmlTag("td");

            new InputModelColumn().WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(typeof (Model1).Name);
            tag.Title().ShouldEqual(typeof (Model1).AssemblyQualifiedName);
        }

        [Test]
        public void write_body_for_chain_with_no_input_type()
        {
            var chain = new BehaviorChain();
            var tag = new HtmlTag("td");

            new InputModelColumn().WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(" -");
        }
    }
}