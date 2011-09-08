using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Diagnostics.Core.Grids;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Features.Requests.Autocomplete
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

        public JsonAutocompleteResultModel Execute(AutocompleteRequestModel<RequestCacheModel> request)
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