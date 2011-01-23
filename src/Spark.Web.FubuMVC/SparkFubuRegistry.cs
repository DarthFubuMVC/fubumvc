using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.UI.Tags;
using Spark.FileSystem;
using Spark.Web.FubuMVC.Registration;
using Spark.Web.FubuMVC.Registration.DSL;
using Spark.Web.FubuMVC.ViewCreation;
using Spark.Web.FubuMVC.ViewLocation;

namespace Spark.Web.FubuMVC
{
    public class SparkFubuRegistry : FubuRegistry
    {
        protected SparkViewFactory Factory;
        private readonly List<ISparkPolicy> _sparkPolicies = new List<ISparkPolicy>();

        public SparkPoliciesExpression SparkPolicies { get { return new SparkPoliciesExpression(_sparkPolicies); } }
        
        public SparkFubuRegistry()
        {
            SetupDefaultConfiguration();
        }

        private void SetupDefaultConfiguration()
        {
            Factory = CreateViewFactory();

            var resolver = new SparkPolicyResolver(_sparkPolicies);

            Services(c =>
            {
                c.AddService(Factory);
                c.AddService(Factory.Settings);
                c.SetServiceIfNone(typeof(ISparkViewRenderer<>), typeof(SparkViewRenderer<>));
            });

            Views
                .Facility(new SparkViewFacility(Factory, resolver))
                .TryToAttach(x => x.by(new ActionAndViewMatchedBySparkViewDescriptors(resolver)));
        }

        public void AddViewFolder(string virtualFolderRoot)
        {
            var settings = (SparkSettings)Factory.Settings;
            settings.AddViewFolder(ViewFolderType.VirtualPathProvider, new Dictionary<string, string> { { "virtualBaseDir", virtualFolderRoot } });
        }

        public virtual SparkViewFactory CreateViewFactory()
        {
            return new SparkViewFactory(CreateSparkSettings());
        }

        public virtual SparkSettings CreateSparkSettings()
        {
            return new SparkSettings()
                .AddAssembly(typeof (PartialTagFactory).Assembly)
                .AddNamespace("Spark.Web.FubuMVC")
                .AddNamespace("FubuMVC.Core.UI")
                .AddNamespace("HtmlTags");
        }
    }
}