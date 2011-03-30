using System.Collections.Generic;
using FubuMVC.Diagnostics.Models.Requests;

namespace FubuMVC.Diagnostics.Grids
{
    public class RequestCacheRowProvider : IGridRowProvider<RequestCacheModel, RecordedRequestModel>
    {
        public IEnumerable<RecordedRequestModel> RowsFor(RequestCacheModel target)
        {
            return target.Requests;
        }
    }
}