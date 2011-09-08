using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Features.Routes
{
    public class RoutesModel : IGridModel
    {
        public JqGridColumnModel ColumnModel { get; set; }
		public JsonGridFilter Filter { get; set; }
    }
}