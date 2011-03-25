using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Endpoints
{
    public class RequestsEndpoint
    {
        private readonly IDebugReport _debugReport;
        private readonly IEnumerable<IGridColumnBuilder<BehaviorReportModel>> _columnBuilders;
        private readonly IDebugReportModelBuilder _reportBuilder;

        public RequestsEndpoint(IDebugReport debugReport, IEnumerable<IGridColumnBuilder<BehaviorReportModel>> columnBuilders, IDebugReportModelBuilder reportBuilder)
        {
            _debugReport = debugReport;
            _reportBuilder = reportBuilder;
            _columnBuilders = columnBuilders;
        }


        public RequestExplorerModel Get(RequestExplorerRequestModel request)
        {
            var report = _reportBuilder.Build(_debugReport);
            // TODO -- move to types? Need to support no data in here
            var columnModel = new JqGridColumnModel();
            var behaviorReport = report.BehaviorReports.FirstOrDefault();
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
            return new RequestExplorerModel();
        }
    }
}