using System;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Grids.Columns;
using FubuMVC.Diagnostics.Models.Grids;

namespace FubuMVC.Diagnostics.Grids.Filters
{
	public abstract class BehaviorChainFilterBase<T> : IGridFilter<BehaviorChain>
		where T : class, IBehaviorChainColumn
	{
		private readonly T _column;

		protected BehaviorChainFilterBase(T column)
		{
			_column = column;

		}

		public T Column { get { return _column; } }

		public virtual bool AppliesTo(BehaviorChain target, JsonGridFilter filter)
		{
			return filter.ColumnName.Equals(_column.Name(), StringComparison.OrdinalIgnoreCase);
		}

		public virtual bool Matches(BehaviorChain target, JsonGridFilter filter)
		{
			return contains(target, filter);
		}

		protected bool startsWith(BehaviorChain target, JsonGridFilter filter)
		{
			return filter.Matches(_column.ValueFor(target), (filterValue, value) => value.StartsWith(filterValue));
		}

		protected bool contains(BehaviorChain target, JsonGridFilter filter)
		{
			return filter.Matches(_column.ValueFor(target), (filterValue, value) => value.Contains(filterValue));
		}
	}
}