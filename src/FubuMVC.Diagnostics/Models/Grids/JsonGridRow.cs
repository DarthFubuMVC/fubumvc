using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Diagnostics.Models.Grids
{
    public class JsonGridRow
    {
        public JsonGridRow()
        {
            Columns = new List<JsonGridColumn>();
        }

        public string Id { get; set; }
        public IEnumerable<JsonGridColumn> Columns { get; set; }

        public JsonGridColumn FindColumn(string columnName)
        {
            if(columnName.IsEmpty())
            {
                return null;
            }

            return Columns.SingleOrDefault(c => c.Name.ToLower() == columnName.ToLower());
        }
    }
}