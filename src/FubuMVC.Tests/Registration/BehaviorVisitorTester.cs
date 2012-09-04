using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class BehaviorVisitorTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            observer = new RecordingConfigurationObserver();
            visitor = new BehaviorVisitor(observer, "reasontovisit");
            chain = new BehaviorChain();
            call = ActionCall.For<TestController>(c => c.SomeAction(null));

            chain.AddToEnd(call);

            processor = MockRepository.GenerateMock<BehaviorProcessor>();

            visitor.Actions += x => processor.Got(x);
        }

        #endregion

        private BehaviorVisitor visitor;
        private BehaviorChain chain;
        private BehaviorProcessor processor;
        private RecordingConfigurationObserver observer;
        private ActionCall call;

        private bool getTrue()
        {
            return true;
        }

        [Test]
        public void should_call_the_inner_processor_if_the_filters_match()
        {
            visitor.Filters += x => true;
            visitor.VisitBehavior(chain);
            processor.AssertWasCalled(x => x.Got(chain));
        }

        [Test]
        public void should_call_the_inner_processor_if_there_are_no_filters()
        {
            visitor.VisitBehavior(chain);
            processor.AssertWasCalled(x => x.Got(chain));
        }

        [Test]
        public void should_not_call_the_inner_processor_if_the_filters_do_not_match()
        {
            visitor.Filters += x => false;
            visitor.VisitBehavior(chain);
            processor.AssertWasNotCalled(x => x.Got(chain));
        }

        // Yes, this is a stupid method, it's used merely for making testing logging easier (see should_log_each_matched_filter())
    }

    public interface BehaviorProcessor
    {
        void Got(BehaviorChain chain);
    }
}