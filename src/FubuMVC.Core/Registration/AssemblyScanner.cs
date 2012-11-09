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
		private readonly CompositeFilter<Type> _typeFilters;
		private readonly List<IServiceRegistrationConvention> _conventions = new List<IServiceRegistrationConvention>();
        private readonly AppliesToExpression _applies = new AppliesToExpression();

		public AssemblyScanner()
		{
			_typeFilters = new CompositeFilter<Type>();
		}

		public AppliesToExpression Applies
		{
			get { return _applies; }
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

		public void Configure(ServiceRegistry services)
		{
		    var types = _applies.BuildPool(TypePool.FindTheCallingAssembly());

			var matchedTypes = types.TypesMatching(_typeFilters.Matches);
			_conventions
				.Each(convention => convention.Register(matchedTypes, services));
		}
	}
}