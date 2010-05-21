using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Tests.Urls;
using HtmlTags;
using NUnit.Framework;

namespace FubuMVC.Tests.Diagnostics
{
    [TestFixture]
    public class ActionColumnTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void write_body_cell_with_multiple_calls()
        {
            var chain = new BehaviorChain();
            ActionCall call1 = ActionCall.For<TargetController>(x => x.Go());
            chain.AddToEnd(call1);

            ActionCall call2 = ActionCall.For<TargetController>(x => x.GoWithInput(null));
            chain.AddToEnd(call2);

            var column = new ActionColumn();

            var tag = new HtmlTag("td");

            column.WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(call1.Description + ", " + call2.Description);
        }

        [Test]
        public void write_body_cell_with_no_calls()
        {
            var chain = new BehaviorChain();

            var column = new ActionColumn();

            var tag = new HtmlTag("td");

            column.WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(" -");
        }

        [Test]
        public void write_body_cell_with_only_one_call()
        {
            var chain = new BehaviorChain();
            ActionCall call = ActionCall.For<TargetController>(x => x.Go());
            chain.AddToEnd(call);

            var column = new ActionColumn();

            var tag = new HtmlTag("td");

            column.WriteBody(chain, null, tag);

            tag.Text().ShouldEqual(call.Description);
        }
    }
}