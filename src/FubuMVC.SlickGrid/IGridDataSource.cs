using System.Collections.Generic;

namespace FubuMVC.SlickGrid
{
    public interface IGridDataSource<T>
    {
        IEnumerable<T> GetData();
    }

    public interface IGridDataSource<T, TQuery>
    {
        IEnumerable<T> GetData(TQuery query);
    }
}