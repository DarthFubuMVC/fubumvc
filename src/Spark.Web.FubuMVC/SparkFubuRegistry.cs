using System;
using System.Collections.Generic;
using FubuMVC.Core;
using Spark.FileSystem;
using Spark.Web.FubuMVC.Registration;
using Spark.Web.FubuMVC.Registration.DSL;
using Spark.Web.FubuMVC.ViewLocation;

namespace Spark.Web.FubuMVC
{
    public class SparkFubuRegistry : FubuRegistry
    {
        protected readonly Func<SparkViewFactory> Factory;
        private readonly List<ISparkPolicy> _sparkPolicies = new List<ISparkPolicy>();

        public SparkPoliciesExpression SparkPolicies { get { return new SparkPoliciesExpression(_sparkPolicies); } }
        
        public SparkFubuRegistry(Func<SparkViewFactory> factory)
        {
            Factory = factory;

            var resolver = new SparkPolicyResolver(_sparkPolicies);

            Views
                .Facility(new SparkViewFacility(Factory(), resolver))
                .TryToAttach(x => x.by(new ActionAndViewMatchedBySparkViewDescriptors(resolver)));
        }

        public void AddViewFolder(string virtualFolderRoot)
        {
            var settings = (SparkSettings)Factory().Settings;
            settings.AddViewFolder(ViewFolderType.VirtualPathProvider, new Dictionary<string, string> { { "virtualBaseDir", virtualFolderRoot } });
        }
    }
}