using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Registration.DSL;

namespace FubuMVC.Core.Registration
{
	public class AssemblyScanner : IAssemblyScanner
	{
		private readonly TypePool _types;
		private readonly CompositeFilter<Type> _typeFilters;
		private readonly List<IServiceRegistrationConvention> _conventions = new List<IServiceRegistrationConvention>();

		public AssemblyScanner()
		{
			_types = new TypePool(GetType().Assembly) { ShouldScanAssemblies = true};
			_typeFilters = new CompositeFilter<Type>();

			IncludeTypes(t => true);
		}

		public AppliesToExpression Applies
		{
			get { return new AppliesToExpression(_types); }
		}

		public IAssemblyScanner IncludeTypes(Expression<Func<Type, bool>> filter)
		{
			_typeFilters.Includes += filter;
			return this;
		}

		public IAssemblyScanner ExcludeTypes(Expression<Func<Type, bool>> filter)
		{
			_typeFilters.Excludes += filter;
			return this;
		}

		public IAssemblyScanner ApplyConvention<T>() where T : IServiceRegistrationConvention, new()
		{
			return ApplyConvention(new T());
		}

		public IAssemblyScanner ApplyConvention(IServiceRegistrationConvention convention)
		{
			_conventions.Fill(convention);
			return this;
		}

		public void Configure(IServiceRegistry services)
		{
			var matchedTypes = _types.TypesMatching(_typeFilters.Matches);
			_conventions
				.Each(convention => convention.Register(matchedTypes, services));
		}
	}
}