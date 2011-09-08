using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Features.Routes
{
    public class get_Column_Value_handler
    {
        private readonly IModelBuilder<RoutesModel> _routeBuilder;

        public get_Column_Value_handler(IModelBuilder<RoutesModel> routeBuilder)
        {
            _routeBuilder = routeBuilder;
        }

        public RoutesModel Execute(RouteRequestModel request)
        {
            var model = _routeBuilder.Build();
            model.Filter = new JsonGridFilter
                               {
                                   ColumnName = request.Column,
                                   Values = new[] {request.Value}
                               };

            return model;
        }
    }
}