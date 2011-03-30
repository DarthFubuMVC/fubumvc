using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration;

namespace FubuMVC.Diagnostics
{
	public interface IServiceRegistrationConvention
	{
		void Register(IEnumerable<Type> matchedTypes, IServiceRegistry services);
	}

	public class AddImplementationsServiceRegistrationConvention : IServiceRegistrationConvention
	{
		private readonly Type _pluginType;

		public AddImplementationsServiceRegistrationConvention(Type pluginType)
		{
			_pluginType = pluginType;
		}

		public void Register(IEnumerable<Type> matchedTypes, IServiceRegistry services)
		{
			matchedTypes
				.Where(t => _pluginType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
				.Each(t => services.FillType(_pluginType, t));
		}
	}

	public class ConnectImplementationsServiceRegistrationConvention : IServiceRegistrationConvention
	{
		private readonly Type _openType;

		public ConnectImplementationsServiceRegistrationConvention(Type openType)
		{
			_openType = openType;
		}

		public void Register(IEnumerable<Type> matchedTypes, IServiceRegistry services)
		{
			matchedTypes
				.Where(t => t.Closes(_openType) && t.IsClass && !t.IsAbstract)
				.Each(t => services.FillType(_openType, t));
		}
	}

	public static class AssemblyScanningExtensions
	{
		public static IAssemblyScanner AddAllTypesOf<T>(this IAssemblyScanner scanner)
		{
			return scanner.AddAllTypesOf(typeof (T));
		}

		public static IAssemblyScanner AddAllTypesOf(this IAssemblyScanner scanner, Type type)
		{
			return scanner.ApplyConvention(new AddImplementationsServiceRegistrationConvention(type));
		}

		public static IAssemblyScanner ConnectImplementationsToTypesClosing(this IAssemblyScanner scanner, Type openType)
		{
			return scanner.ApplyConvention(new ConnectImplementationsServiceRegistrationConvention(openType));
		}
	}
}