using System.Collections.Generic;
using System.Linq;
using FubuMVC.Diagnostics.Grids.Columns;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids
{
	public class GridColumnBuilder<T> : IGridColumnBuilder<T>
	{
		private readonly IEnumerable<IGridColumn<T>> _columns;

		public GridColumnBuilder(IEnumerable<IGridColumn<T>> columns)
		{
			_columns = columns;
		}

		public IEnumerable<JsonGridColumn> ColumnsFor(T target)
		{
			return _columns.Select(col => new JsonGridColumn
			                              	{
			                              		Name = col.Name(),
			                              		Value = col.ValueFor(target),
			                              		IsHidden = col.IsHidden(target),
			                              		HideFilter = col.HideFilter(target),
			                              		IsIdentifier = col.IsIdentifier()
			                              	});
		}
	}
}