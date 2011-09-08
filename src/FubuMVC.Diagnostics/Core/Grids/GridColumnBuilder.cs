using System.Collections.Generic;
using System.Linq;
using FubuMVC.Diagnostics.Core.Grids.Columns;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Core.Grids
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
			return _columns
				.OrderByDescending(c => c.Rank())
				.Select(col => new JsonGridColumn
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