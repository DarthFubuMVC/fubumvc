using System.Collections.Generic;
using FubuMVC.Diagnostics.Features.Requests;

namespace FubuMVC.Diagnostics.Core.Grids
{
    public class RequestCacheRowProvider : IGridRowProvider<RequestCacheModel, RecordedRequestModel>
    {
        public IEnumerable<RecordedRequestModel> RowsFor(RequestCacheModel target)
        {
            return target.Requests;
        }
    }
}