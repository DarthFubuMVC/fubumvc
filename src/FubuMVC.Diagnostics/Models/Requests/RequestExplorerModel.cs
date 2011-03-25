using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Models.Requests
{
    public class RequestExplorerModel : IGridModel
    {
        public JqGridColumnModel ColumnModel { get; set; }
    }
}