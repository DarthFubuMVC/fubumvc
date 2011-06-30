using System.Collections.Generic;
using System.Linq;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Endpoints
{
    public class RequestsEndpoint
    {
        private readonly IEnumerable<IGridColumnBuilder<RecordedRequestModel>> _columnBuilders;
        private readonly IRequestCacheModelBuilder _reportBuilder;

        public RequestsEndpoint(IEnumerable<IGridColumnBuilder<RecordedRequestModel>> columnBuilders, IRequestCacheModelBuilder reportBuilder)
        {
            _reportBuilder = reportBuilder;
            _columnBuilders = columnBuilders;
        }


        public RequestExplorerModel Get(RequestExplorerRequestModel request)
        {
            var report = _reportBuilder.Build();
            
			// TODO -- move to types? Need to support no data in here
            var columnModel = new JqGridColumnModel();
            var behaviorReport = report.Requests.FirstOrDefault();

            if (behaviorReport != null)
            {
                _columnBuilders
                    .SelectMany(builder => builder.ColumnsFor(behaviorReport))
                    .Each(col => columnModel.AddColumn(new JqGridColumn
                                                           {
                                                               hidden = col.IsHidden,
                                                               hidedlg = col.IsHidden,
                                                               hideFilter = col.HideFilter,
                                                               name = col.Name,
                                                               index = col.Name
                                                           }));
            }
            return new RequestExplorerModel { ColumnModel = columnModel };
        }
    }
}