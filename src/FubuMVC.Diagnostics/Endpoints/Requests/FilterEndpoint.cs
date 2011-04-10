using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Endpoints.Requests
{
	public class FilterEndpoint
	{
		private readonly IGridService<RequestCacheModel, RecordedRequestModel> _gridService;
		private readonly IRequestCacheModelBuilder _modelBuilder;

		public FilterEndpoint(IGridService<RequestCacheModel, RecordedRequestModel> gridService, IRequestCacheModelBuilder modelBuilder)
		{
			_modelBuilder = modelBuilder;
			_gridService = gridService;
		}

		public JsonGridModel Post(JsonGridQuery<RequestCacheModel> query)
		{
			var model = _modelBuilder.Build();
			return _gridService.GridFor(model, query);
		}
	}
}