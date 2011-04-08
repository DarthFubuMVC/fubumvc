using System;
using FubuMVC.Diagnostics.Grids.Columns;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids.Filters
{
	public abstract class GridFilterBase<TColumn, TModel> : IGridFilter<TModel>
		where TColumn : class, IGridColumn<TModel>
	{
		private readonly TColumn _column;

		protected GridFilterBase(TColumn column)
		{
			_column = column;

		}

		public TColumn Column { get { return _column; } }

		public virtual bool AppliesTo(TModel target, JsonGridFilter filter)
		{
			return filter.ColumnName.Equals(_column.Name(), StringComparison.OrdinalIgnoreCase);
		}

		public virtual bool Matches(TModel target, JsonGridFilter filter)
		{
			return contains(target, filter);
		}

		protected bool startsWith(TModel target, JsonGridFilter filter)
		{
			return filter.Matches(_column.ValueFor(target), (filterValue, value) => value.StartsWith(filterValue));
		}

		protected bool contains(TModel target, JsonGridFilter filter)
		{
			return filter.Matches(_column.ValueFor(target), (filterValue, value) => value.Contains(filterValue));
		}
	}
}