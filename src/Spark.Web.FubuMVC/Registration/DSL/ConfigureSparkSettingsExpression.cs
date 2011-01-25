using System.Collections.Generic;
using System.Reflection;
using Spark.FileSystem;

namespace Spark.Web.FubuMVC.Registration.DSL
{
	public class ConfigureSparkSettingsExpression
	{
		private readonly SparkSettings _settings;

		public ConfigureSparkSettingsExpression(SparkSettings settings)
		{
			_settings = settings;
		}

		public ConfigureSparkSettingsExpression AddAssemblyContainingType<T>()
		{
			return AddAssembly(typeof (T).Assembly);
		}

		public ConfigureSparkSettingsExpression AddAssembly(Assembly assembly)
		{
			_settings.AddAssembly(assembly);
			return this;
		}

		public ConfigureSparkSettingsExpression AddAssembly(string assembly)
		{
			_settings.AddAssembly(assembly);
			return this;
		}

		public ConfigureSparkSettingsExpression AddNamespace(string @namespace)
		{
			_settings.AddNamespace(@namespace);
			return this;
		}

		public ConfigureSparkSettingsExpression AddViewFolder(string virtualFolderRoot)
		{
			_settings.AddViewFolder(ViewFolderType.VirtualPathProvider, new Dictionary<string, string> { { "virtualBaseDir", virtualFolderRoot } });
			return this;
		}
	}
}