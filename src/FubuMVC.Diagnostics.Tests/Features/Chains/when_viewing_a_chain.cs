using System;
using FubuMVC.Diagnostics.Features.Chains;
using FubuMVC.Diagnostics.Features.Chains.View;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Features.Chains
{
	[TestFixture]
	public class when_viewing_a_chain : InteractionContext<get_Id_handler>
	{
        [Test]
        public void should_throw_argument_exception_if_chain_cannot_be_found()
        {
            var id = Guid.NewGuid();

            MockFor<IChainVisualizerBuilder>()
                .Expect(b => b.VisualizerFor(id))
                .Return(null);

            Exception<ArgumentException>
                .ShouldBeThrownBy(() => ClassUnderTest.Execute(new ChainRequest { Id = id }));
        }

        [Test]
        public void should_build_from_model_builder()
        {
            var id = Guid.NewGuid();
            var visualizer = new ChainModel();

            MockFor<IChainVisualizerBuilder>()
                .Expect(b => b.VisualizerFor(id))
                .Return(visualizer);

            ClassUnderTest
                .Execute(new ChainRequest { Id = id })
                .ShouldEqual(visualizer);

        }
	}
}