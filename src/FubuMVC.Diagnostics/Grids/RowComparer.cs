using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids
{
    public class RowComparer
    {
        public static int CompareRows(string columnName, JsonGridRow x, JsonGridRow y)
        {
            var xCol = x.FindColumn(columnName);
            var yCol = y.FindColumn(columnName);
            if (xCol == null || yCol == null)
            {
                return 0;
            }

            return xCol.Value.CompareTo(yCol.Value);
        }
    }
}