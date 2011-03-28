using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Endpoints.Requests
{
	public class FilterEndpoint
	{
		private readonly IDebugReport _debugReport;
		private readonly IGridService<DebugReportModel, BehaviorReportModel> _gridService;
		private readonly IDebugReportModelBuilder _modelBuilder;

		public FilterEndpoint(IDebugReport debugReport, IGridService<DebugReportModel, BehaviorReportModel> gridService, IDebugReportModelBuilder modelBuilder)
		{
			_debugReport = debugReport;
			_modelBuilder = modelBuilder;
			_gridService = gridService;
		}

		public JsonGridModel Post(JsonGridQuery<DebugReportModel> query)
		{
			var model = _modelBuilder.Build(_debugReport);
			return _gridService.GridFor(model, query);
		}
	}
}