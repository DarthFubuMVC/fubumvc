using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids
{
    public interface IGridService<T>
    {
        JsonGridModel GridFor(T target, JsonGridQuery query);
    }
}