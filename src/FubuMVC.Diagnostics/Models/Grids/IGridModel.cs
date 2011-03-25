using FubuMVC.Diagnostics.Partials;

namespace FubuMVC.Diagnostics.Models.Grids
{
    public interface IGridModel : IPartialModel
    {
        JqGridColumnModel ColumnModel { get; set; }
    }
}