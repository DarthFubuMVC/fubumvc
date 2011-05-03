using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration
{
	public interface IServiceRegistrationConvention
	{
		void Register(IEnumerable<Type> matchedTypes, IServiceRegistry services);
	}
}