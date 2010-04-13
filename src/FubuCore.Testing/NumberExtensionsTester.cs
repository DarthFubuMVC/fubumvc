using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing
{
    [TestFixture]
    public class NumberExtensionsTester
    {
        private IAction _action;

        [SetUp]
        public void SetUp()
        {
            _action = MockRepository.GenerateStub<IAction>();
            _action.Stub(a => a.DoSomething(Arg<int>.Is.LessThan(6))).Repeat.Times(6);
        }

        [Test]
        public void Times_runs_an_action_the_specified_number_of_times()
        {
            int maxCount = 6;
            maxCount.Times(_action.DoSomething);
            _action.AssertWasCalled(a => a.DoSomething(Arg<int>.Is.LessThan(6)), c => c.Repeat.Times(6));
        }

        public interface IAction
        {
            void DoSomething(int index);
        }
    }
}