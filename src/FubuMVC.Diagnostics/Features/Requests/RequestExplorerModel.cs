using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Features.Requests
{
    public class RequestExplorerModel : IGridModel
    {
        public JqGridColumnModel ColumnModel { get; set; }
    }
}