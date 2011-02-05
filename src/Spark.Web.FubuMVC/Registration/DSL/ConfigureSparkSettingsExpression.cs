using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Spark.FileSystem;

namespace Spark.Web.FubuMVC.Registration.DSL
{
	public class ConfigureSparkSettingsExpression
	{
		public static readonly string ViewFolderParam = "virtualBaseDir";
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
			if (!_settings.ViewFolders.Any(f => f.Parameters.ContainsKey(ViewFolderParam) && f.Parameters[ViewFolderParam] == virtualFolderRoot))
			{
				_settings.AddViewFolder(ViewFolderType.VirtualPathProvider, new Dictionary<string, string> { { ViewFolderParam, virtualFolderRoot } });
			}

			return this;
		}
	}
}