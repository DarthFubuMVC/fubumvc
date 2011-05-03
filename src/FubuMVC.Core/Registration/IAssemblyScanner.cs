using System;
using System.Linq.Expressions;
using FubuMVC.Core.Registration.DSL;

namespace FubuMVC.Core.Registration
{
	public interface IAssemblyScanner
	{
		AppliesToExpression Applies { get; }
		IAssemblyScanner IncludeTypes(Expression<Func<Type, bool>> filter);
		IAssemblyScanner ExcludeTypes(Expression<Func<Type, bool>> filter);

		IAssemblyScanner ApplyConvention<T>()
			where T : IServiceRegistrationConvention, new();

		IAssemblyScanner ApplyConvention(IServiceRegistrationConvention convention);
	}
}