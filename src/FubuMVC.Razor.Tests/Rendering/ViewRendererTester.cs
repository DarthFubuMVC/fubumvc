using System.Collections.Generic;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Razor.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.Rendering
{
    [TestFixture]
    public class ViewRendererTester : InteractionContext<ViewRenderer>
    {
        private IRenderStrategy[] _strategies;

        protected override void beforeEach()
        {
            _strategies = Services.CreateMockArrayFor<IRenderStrategy>(3);
            _strategies[0].Expect(x => x.Applies()).Return(false);
            _strategies[1].Expect(x => x.Applies()).Return(true);
            _strategies[2].Expect(x => x.Applies()).Return(false).Repeat.Never();

            _strategies[0].Expect(x => x.Invoke(Arg<IRenderAction>.Is.Anything)).Repeat.Never();
            _strategies[1].Expect(x => x.Invoke(Arg<IRenderAction>.Is.Anything));
            _strategies[2].Expect(x => x.Invoke(Arg<IRenderAction>.Is.Anything)).Repeat.Never();
        }

        [Test]
        public void when_rendering_the_first_applicable_strategy_is_invoked()
        {
            ClassUnderTest.Render();
            _strategies.Each(x => x.VerifyAllExpectations());
        }
    }
}