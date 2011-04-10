using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids
{
    public interface IGridService<TModel, TRow>
    {
        JsonGridModel GridFor(TModel target, JsonGridQuery query);
    }
}