using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Models.Routes
{
    public class RoutesModel : IGridModel
    {
        public JqGridColumnModel ColumnModel { get; set; }
		public JsonGridFilter Filter { get; set; }
    }
}