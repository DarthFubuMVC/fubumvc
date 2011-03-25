using System.Collections.Generic;
using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Grids
{
    public class DebugReportRowProvider : IGridRowProvider<DebugReportModel, BehaviorReportModel>
    {
        public IEnumerable<BehaviorReportModel> RowsFor(DebugReportModel target)
        {
            return target.BehaviorReports;
        }
    }
}