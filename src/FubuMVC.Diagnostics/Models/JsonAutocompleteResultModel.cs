using System.Collections.Generic;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Models
{
    public class JsonAutocompleteResultModel
    {
        public JsonAutocompleteResultModel()
        {
            Values = new List<JsonGridColumn>();
        }

        public IEnumerable<JsonGridColumn> Values { get; set; }
    }
}