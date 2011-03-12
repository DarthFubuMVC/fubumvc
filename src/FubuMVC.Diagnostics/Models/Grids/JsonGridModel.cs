using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Grids
{
    // jqGrid-specific format
    public class JsonGridModel
    {
        public int page { get; set; }
        public int total { get; set; }
        public int records { get; set; }
        public IEnumerable<JsonGridRow> rows { get; set; }
    }
}