using FubuTestingSupport;
using NUnit.Framework;
using FubuMVC.Core.Continuations;

namespace FubuMVC.Tests.Continuations
{
    [TestFixture]
    public class RedirectableTester
    {
        [Test]
        public void Can_create_from_model()
        {
            var model = new Model { SomeString = "Test" };
            var redirectable = Redirectable.FromModel(model);
            redirectable.ShouldNotBeNull();
            redirectable.Model.ShouldBeTheSameAs(model);
        }

        [Test]
        public void Has_nextbehaviour_continuation_per_default()
        {
            var redirectable = new Redirectable<Model>(new Model());
            redirectable.Continuation.AssertWasContinuedToNextBehavior();
        }

        [Test]
        public void Can_set_continuation()
        {
            var continuation = FubuContinuation.RedirectTo(new Model());
            var redirectable = new Redirectable<Model>(continuation);
            redirectable.Continuation.ShouldBeTheSameAs(continuation);
        }
    }

    public class Model
    {
        public string SomeString { get; set; }
    }
}
