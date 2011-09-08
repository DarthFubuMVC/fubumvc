using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Features.Requests.Filter
{
	public class PostHandler
	{
		private readonly IGridService<RequestCacheModel, RecordedRequestModel> _gridService;
		private readonly IRequestCacheModelBuilder _modelBuilder;

		public PostHandler(IGridService<RequestCacheModel, RecordedRequestModel> gridService, IRequestCacheModelBuilder modelBuilder)
		{
			_modelBuilder = modelBuilder;
			_gridService = gridService;
		}

		public JsonGridModel Execute(JsonGridQuery<RequestCacheModel> query)
		{
			var model = _modelBuilder.Build();
			return _gridService.GridFor(model, query);
		}
	}
}