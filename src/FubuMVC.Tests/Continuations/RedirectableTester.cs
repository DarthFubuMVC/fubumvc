using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Model model = new Model() { SomeString = "Test" };
            Redirectable<Model> redirectable = Redirectable.FromModel(model);
            redirectable.ShouldNotBeNull();
            redirectable.Model.ShouldBeTheSameAs(model);
        }

        [Test]
        public void Has_nextbehaviour_continuation_per_default()
        {
            Redirectable<Model> redirectable = new Redirectable<Model>();
            redirectable.Continuation.AssertWasContinuedToNextBehavior();
        }

        [Test]
        public void Can_set_continuation()
        {
            Redirectable<Model> redirectable = new Redirectable<Model>();
            FubuContinuation continuation = FubuContinuation.RedirectTo(new Model());
            redirectable.Continuation = continuation;
            redirectable.Continuation.ShouldBeTheSameAs(continuation);
        }
    }

    public class Model
    {
        public string SomeString { get; set; }
    }
}
