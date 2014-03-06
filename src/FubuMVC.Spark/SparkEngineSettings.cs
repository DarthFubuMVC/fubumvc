using System;
using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core;
using Spark;

namespace FubuMVC.Spark
{
    public class SparkEngineSettings
    {
        private Func<bool> _precompileViews;

        public SparkEngineSettings()
        {
            defaultSearch();

            _precompileViews = () => !FubuMode.InDevelopment();
        }

        private void defaultSearch()
        {
            Search = new FileSet {DeepSearch = true};
            Search.AppendInclude("*{0}".ToFormat(Constants.DotSpark));
            Search.AppendInclude("*{0}".ToFormat(Constants.DotShade));
            Search.AppendInclude("bindings.xml");
            Search.AppendExclude("bin/*.*");
            Search.AppendExclude("obj/*.*");
        }


        /// <summary>
        /// List of namespaces that will be applied as global namespaces to the SparkViewEngine
        /// </summary>
        public readonly IList<string> UseNamespaces = new List<string>();

        /// <summary>
        /// Adds a namespace to UseNamespaces
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UseNamespaceIncludingType<T>()
        {
            UseNamespaces.Add(typeof (T).Name);
        }

        public FileSet Search { get; private set; }

        /// <summary>
        /// By default, FubuMVC.Spark will precompile views if FubuMode.InDevelopment() is false.
        /// </summary>
        public bool PrecompileViews
        {
            get { return _precompileViews(); }
            set { _precompileViews = () => value; }
        }
    }
}