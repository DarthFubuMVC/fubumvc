using System;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Grids.Columns
{
	public abstract class BehaviorChainColumnBase : IBehaviorChainColumn
	{
		private readonly string _name;

		protected BehaviorChainColumnBase(string name)
		{
			_name = name;
		}

		protected BehaviorChainColumnBase(Expression<Func<BehaviorChain, object>> expression)
			: this((string) expression.ToAccessor().Name)
		{
		}

		public virtual string Name()
		{
			return _name;
		}

		public abstract string ValueFor(BehaviorChain chain);
		public abstract bool IsIdentifier();
		public abstract bool IsHidden(BehaviorChain chain);
		public abstract bool HideFilter(BehaviorChain chain);
	}
}