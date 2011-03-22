using System.Collections.Generic;

namespace FubuMVC.Diagnostics.Models.Grids
{
    public class JsonGridFilter
    {
        public string ColumnName { get; set; }
        public IEnumerable<string> Values { get; set; }
    }
}