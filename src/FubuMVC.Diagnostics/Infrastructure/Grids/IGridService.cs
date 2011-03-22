using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Infrastructure.Grids
{
    public interface IGridService<T>
    {
        JsonGridModel GridFor(T target, JsonGridQuery query);
    }
}