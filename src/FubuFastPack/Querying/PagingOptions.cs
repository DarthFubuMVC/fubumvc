using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuFastPack.Querying
{
    public class PagingOptions
    {
        public PagingOptions(int page, int rows, string sidx, bool sord)
        {
            this.page = page;
            this.rows = rows;
            this.sidx = sidx;
            this.sord = sord ? "asc" : "desc";
        }


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
            return new GridDataRequest(page, rows, sidx, sortAscending)
                   {
                       Criterion = criterion == null ? new Criteria[0] : criterion.ToArray()
                   };
        }

        public PagingOptions()
        {
        }


    }
}