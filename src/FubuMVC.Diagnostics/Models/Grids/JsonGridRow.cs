using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Grids
{
    // jqGrid-specific format
    public class JsonGridRow
    {
        public string id { get; set; }
        public IEnumerable<string> cell { get; set; }
    }
}