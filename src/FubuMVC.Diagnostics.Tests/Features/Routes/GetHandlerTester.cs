using FubuMVC.Diagnostics.Features.Routes;
using FubuMVC.Diagnostics.Models;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Features.Routes
{
    [TestFixture]
    public class GetHandlerTester : InteractionContext<GetHandler>
    {
        [Test]
        public void should_build_route_model_with_defaults()
        {
            var model = new RoutesModel();
            MockFor<IModelBuilder<RoutesModel>>()
                .Expect(b => b.Build())
                .Return(model);

            ClassUnderTest
                .Execute(new DefaultRouteRequestModel())
                .ShouldEqual(model);
        }
    }
}