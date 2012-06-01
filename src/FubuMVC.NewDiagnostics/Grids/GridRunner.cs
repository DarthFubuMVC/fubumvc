using System.Collections.Generic;

namespace FubuMVC.NewDiagnostics.Grids
{

    public interface IGridDataSource<T>
    {
        IEnumerable<T> GetData();
    }

    public interface IGridDataSource<T, TQuery>
    {
        IEnumerable<T> GetData(TQuery query);
    }

    public class GridRunner<T, TGrid, TDataSource>
        where TGrid : IGridDefinition<T>
        where TDataSource : IGridDataSource<T>
    {
        private readonly TGrid _grid;
        private readonly TDataSource _source;

        public GridRunner(TGrid grid, TDataSource source)
        {
            _grid = grid;
            _source = source;
        }

        public IEnumerable<IDictionary<string, object>> Run()
        {
            var data = _source.GetData();
            return _grid.FormatData(data);
        }
    }

    public class GridRunner<T, TGrid, TDataSource, TQuery>
        where TGrid : IGridDefinition<T>
        where TDataSource : IGridDataSource<T, TQuery>        
    {
        private readonly TGrid _grid;
        private readonly TDataSource _source;

        public GridRunner(TGrid grid, TDataSource source)
        {
            _grid = grid;
            _source = source;
        }

        public IEnumerable<IDictionary<string, object>> Run(TQuery query)
        {
            var data = _source.GetData(query);
            return _grid.FormatData(data);
        }
    }
}