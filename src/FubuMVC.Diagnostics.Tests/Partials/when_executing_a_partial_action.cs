using System;
using FubuMVC.Diagnostics.Partials;
using FubuMVC.Tests;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Partials
{
    [TestFixture]
    public class when_executing_a_partial_action : InteractionContext<PartialAction<TestPartialModel>>
    {
        [Test]
        public void should_enrich_model_with_decorators()
        {
            var input = new TestPartialModel();
            var output = new TestPartialModel {Id = Guid.NewGuid()};
            
            MockFor<IPartialDecorator<TestPartialModel>>()
                .Expect(dec => dec.Enrich(input))
                .Return(output);

            ClassUnderTest
                .Execute(input)
                .ShouldEqual(output);
        }
    }

    public class TestPartialModel : IPartialModel
    {
        public Guid Id { get; set; }
    }
}