using System;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public abstract class GridColumnBase<T> : IGridColumn<T>
	{
		private readonly string _name;
		private readonly Accessor _accessor;

		protected GridColumnBase(string name)
		{
			_name = name;
		}

		protected GridColumnBase(Expression<Func<T, object>> expression)
			: this(expression.ToAccessor().Name)
		{
			_accessor = expression.ToAccessor();
		}

		public virtual string Name()
		{
			return _name;
		}

		public virtual string ValueFor(T target)
		{
			if(_accessor != null)
			{
				var value = _accessor.GetValue(target);
				return value == null ? string.Empty : value.ToString();
			}

			throw new InvalidOperationException("No accessor defined for {0}".ToFormat(target.GetType().Name));
		}

		public virtual bool IsIdentifier()
		{
			return false;
		}
		
		public virtual bool IsHidden(T target)
		{
			return false;
		}

		public virtual bool HideFilter(T target)
		{
			return false;
		}
	}
}