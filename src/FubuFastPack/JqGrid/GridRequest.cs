using System;
using System.Collections.Generic;
using System.Linq;
using FubuFastPack.Querying;

namespace FubuFastPack.JqGrid
{
    public class GridRequest<T> where T : ISmartGrid
    {
        public int page { get; set; }
        public int rows { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
// ReSharper disable InconsistentNaming
        public string _search { get; set; }
// ReSharper restore InconsistentNaming
        public string nd { get; set; }

        public IList<Criteria> criterion { get; set; }
        public string gridName { get; set; }

        // TODO -- put a UT around this.

        public GridDataRequest ToDataRequest()
        {
            var sortAscending = !"desc".Equals(sord, StringComparison.OrdinalIgnoreCase);
            return new GridDataRequest(page, rows, sidx, sortAscending){
                Criterion = criterion == null ? new Criteria[0] : criterion.ToArray()
            };
        }
    }
}