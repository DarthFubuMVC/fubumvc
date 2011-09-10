using FubuMVC.Diagnostics.Features.Routes;
using FubuMVC.Diagnostics.Models;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Diagnostics.Tests.Features.Routes
{
    [TestFixture]
    public class get_Column_Value_handler_Tester : InteractionContext<get_Column_Value_handler>
    {
        [Test]
        public void should_build_route_model_with_filters()
        {
            var model = new RoutesModel();
            MockFor<IModelBuilder<RoutesModel>>()
                .Expect(b => b.Build())
                .Return(model);

            var request = new RouteRequestModel
                              {
                                  Column = "test",
                                  Value = "123"
                              };

            var filter = ClassUnderTest
                .Execute(request)
                .Filter;

            filter
                .ColumnName
                .ShouldEqual(request.Column);

            filter
                .Values
                .ShouldContain(request.Value);
        }
    }
}