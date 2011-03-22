using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Grids
{
    public class JsonGridModel
    {
        public JsonGridModel()
        {
            Rows = new List<JsonGridRow>();
        }

        public int PageNr { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public IEnumerable<JsonGridRow> Rows { get; set; }
    }
}