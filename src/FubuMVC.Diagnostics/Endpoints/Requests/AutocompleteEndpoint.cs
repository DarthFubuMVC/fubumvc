using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;
using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Endpoints.Requests
{
    public class AutocompleteEndpoint
    {
		private readonly IGridService<RequestCacheModel, RecordedRequestModel> _gridService;
		private readonly IRequestCacheModelBuilder _modelBuilder;

		public AutocompleteEndpoint(IGridService<RequestCacheModel, RecordedRequestModel> gridService, IRequestCacheModelBuilder modelBuilder)
		{
			_modelBuilder = modelBuilder;
			_gridService = gridService;
		}

        public JsonAutocompleteResultModel Post(AutocompleteRequestModel<RequestCacheModel> request)
        {
        	var model = _modelBuilder.Build();
			var filter = new JsonGridFilter { ColumnName = request.Column, Values = new List<string> { request.Query } };
			var query = JsonGridQuery.ForFilter(filter);
			return new JsonAutocompleteResultModel
			{
				Values = _gridService
					.GridFor(model, query)
					.Rows
					.SelectMany(r => r.Columns.Where(c => c.Name.Equals(request.Column, StringComparison.OrdinalIgnoreCase)))
					.Distinct()
			};
        }
    }
}