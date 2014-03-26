using System;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime.Logging;
using FubuMVC.Diagnostics.Requests;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Diagnostics.Runtime.Tracing;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.Requests
{
    [TestFixture]
    public class BehaviorNodeTraceTagTester
    {
        private RequestLog theLog;
        private FakeNode theNode;
        private Lazy<HtmlTag> _tag;

        [SetUp]
        public void SetUp()
        {
            theLog = new RequestLog();
            theNode = new FakeNode();

            _tag = new Lazy<HtmlTag>(() =>
            {
                return new BehaviorNodeTraceTag(theNode, theLog);
            });
        }

        private HtmlTag theResultingTag
        {
            get
            {
                return _tag.Value;
            }
        }

        private void logStartAndFinish(double start, double finish)
        {
            var correlation = new BehaviorCorrelation(theNode);

            theLog.AddLog(start, new BehaviorStart(correlation));
            theLog.AddLog(finish, new BehaviorFinish(correlation));
        }

        private void hasAnExceptionAgainstTheNode()
        {
            var correlation = new BehaviorCorrelation(theNode);

            
            
            theLog.AddLog(10, new BehaviorStart(correlation));
            theLog.AddLog(12, new ExceptionReport("bad", new Exception()){CorrelationId = theNode.UniqueId});
            theLog.AddLog(15, new BehaviorFinish(correlation));
        }

        private void anotherNodeFailed()
        {
            theLog.AddLog(12, new ExceptionReport("bad", new Exception()) { CorrelationId = Guid.NewGuid() });
        }

        [Test]
        public void the_outermost_tag_is_an_li()
        {
            theResultingTag.TagName().ShouldEqual("li");
        }

        [Test]
        public void places_the_behavior_title()
        {
            var expectedTitle = Description.For(new FakeNode()).Title;

            theResultingTag.FirstChild().Text().ShouldEqual(expectedTitle);
        }

        [Test]
        public void the_behavior_did_not_run_so_the_tag_is_shown_as_grayed_out()
        {
            theResultingTag.HasClass("gray");
        }

        [Test]
        public void if_the_behavior_ran_with_no_related_exceptions_no_red_no_gray()
        {
            anotherNodeFailed();
            logStartAndFinish(15, 22);

            theResultingTag.HasClass("gray").ShouldBeFalse();
            theResultingTag.HasClass("exception").ShouldBeFalse();
        }

        [Test]
        public void the_tag_is_marked_as_exception_if_the_behavior_ran_with_an_exception()
        {
            hasAnExceptionAgainstTheNode();

            theResultingTag.HasClass("exception").ShouldBeTrue();
            theResultingTag.HasClass("gray").ShouldBeFalse();
        }

        [Test]
        public void if_the_behavior_ran_place_the_duration_in_the_tag()
        {
            logStartAndFinish(15, 22);

            theResultingTag.ToString().ShouldContain("<span class=\"node-trace-duration\">7 ms</span>");
        }

        [Test]
        public void do_not_try_to_show_the_duration_if_the_behavior_did_not_run()
        {
            theResultingTag.ToString().ShouldNotContain("node-trace-duration");
        }
    }

    [Title("I'm the fake node")]
    public class FakeNode : BehaviorNode
    {
        protected override ObjectDef buildObjectDef()
        {
            throw new NotImplementedException();
        }

        public override BehaviorCategory Category
        {
            get { throw new NotImplementedException(); }
        }
    }
}