using System.Collections.Generic;
using System.Linq;
using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Assets.Combination;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;
using FubuMVC.Core.Assets.Http;
using FubuMVC.Core.Assets.Tags;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Resources.Etags;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Assets
{
    [TestFixture]
    public class AssetServicesRegistrationTester
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            new FubuRegistry().BuildGraph().Services.DefaultServiceFor<TService>().Type.ShouldEqual(
                typeof(TImplementation));
        }

        [Test]
        public void ContentPipeline_is_registered()
        {
            registeredTypeIs<IContentPipeline, ContentPipeline>();
        }

        [Test]
        public void asset_combination_cache_is_registered_as_a_singleton()
        {
            registeredTypeIs<IAssetCombinationCache, AssetCombinationCache>();
            ServiceRegistry.ShouldBeSingleton(typeof(AssetCombinationCache)).ShouldBeTrue();
        }

        [Test]
        public void asset_dependency_finder_should_be_registered_as_a_singleton()
        {
            registeredTypeIs<IAssetDependencyFinder, AssetDependencyFinderCache>();
            ServiceRegistry.ShouldBeSingleton(typeof(AssetDependencyFinderCache))
                .ShouldBeTrue();
        }

        [Test]
        public void asset_graph_and_pipeline_activators_are_registered_in_the_correct_order()
        {
            var activators = new FubuRegistry().BuildGraph().Services.ServicesFor<IActivator>().ToList();

            activators.Any(x => x.Type == typeof(AssetGraphConfigurationActivator)).ShouldBeTrue();
            activators.Any(x => x.Type == typeof(AssetPipelineBuilderActivator)).ShouldBeTrue();
            activators.Any(x => x.Type == typeof(AssetDeclarationVerificationActivator)).ShouldBeTrue();
            activators.Any(x => x.Type == typeof(AssetPolicyActivator)).ShouldBeTrue();

            activators.RemoveAll(x => !x.Type.Namespace.Contains(typeof(AssetGraph).Namespace));

            activators[0].Type.ShouldEqual(typeof(AssetPipelineBuilderActivator));
            activators[1].Type.ShouldEqual(typeof(AssetPrecompilerActivator));
            activators[2].Type.ShouldEqual(typeof(AssetGraphConfigurationActivator));
            activators[3].Type.ShouldEqual(typeof(AssetDeclarationVerificationActivator));
            activators[4].Type.ShouldEqual(typeof(MimetypeRegistrationActivator));
            activators[5].Type.ShouldEqual(typeof(AssetCombinationBuildingActivator));
            activators[6].Type.ShouldEqual(typeof(AssetPolicyActivator));
            activators[7].Type.ShouldEqual(typeof(AssetFileWatchingActivator));

        }

        [Test]
        public void asset_pipeline_is_registered_as_both_IAssetPipeline_and_IAssetFileRegistration_as_the_same_instance()
        {
            var services = new FubuRegistry().BuildGraph().Services;
            var pipeline1 = services.DefaultServiceFor<IAssetPipeline>().Value.ShouldBeOfType<AssetPipeline>();
            var pipeline2 = services.DefaultServiceFor<IAssetFileRegistration>().Value.ShouldBeOfType<AssetPipeline>();

            pipeline1.ShouldNotBeNull();
            pipeline2.ShouldNotBeNull();

            pipeline1.ShouldBeTheSameAs(pipeline2);
        }

        [Test]
        public void asset_requirements_are_registered()
        {
            registeredTypeIs<IAssetRequirements, AssetRequirements>();
        }

        [Test]
        public void asset_tag_builder_is_registered()
        {
            registeredTypeIs<IAssetTagBuilder, AssetTagBuilder>();
        }

        [Test]
        public void asset_tag_plan_cache_is_registered_as_a_singleton()
        {
            registeredTypeIs<IAssetTagPlanCache, AssetTagPlanCache>();
            ServiceRegistry.ShouldBeSingleton(typeof(AssetTagPlanCache)).ShouldBeTrue();
        }

        [Test]
        public void asset_tag_planner_is_registered()
        {
            registeredTypeIs<IAssetTagPlanner, AssetTagPlanner>();
        }

        [Test]
        public void asset_tag_write_is_registered()
        {
            registeredTypeIs<IAssetTagWriter, AssetTagWriter>();
        }

        [Test]
        public void by_default_the_missing_asset_handler_is_traceonle()
        {
            registeredTypeIs<IMissingAssetHandler, TraceOnlyMissingAssetHandler>();
        }

        [Test]
        public void combination_determination_service_is_registered()
        {
            registeredTypeIs<ICombinationDeterminationService, CombinationDeterminationService>();
        }

        [Test]
        public void content_plan_cache_is_registered_as_a_singleton()
        {
            registeredTypeIs<IContentPlanCache, ContentPlanCache>();
            ServiceRegistry.ShouldBeSingleton(typeof(ContentPlanCache));
        }

        [Test]
        public void content_planner_is_registered()
        {
            registeredTypeIs<IContentPlanner, ContentPlanner>();
        }

        [Test]
        public void iscriptwriter_is_registered_to_the_basic_writer()
        {
            registeredTypeIs<IAssetTagWriter, AssetTagWriter>();
        }

        [Test]
        public void script_graph_is_registered()
        {
            new FubuRegistry().BuildGraph().Services.DefaultServiceFor<AssetGraph>().Value.ShouldNotBeNull();
        }

        [Test]
        public void should_be_a_script_configuration_activator_registered_as_a_service()
        {
            new FubuRegistry().BuildGraph().Services.ServicesFor<IActivator>()
                .Any(x => x.Type == typeof(AssetGraphConfigurationActivator)).ShouldBeTrue();
        }

        [Test]
        public void transformer_library_is_registered()
        {
            registeredTypeIs<ITransformerPolicyLibrary, TransformerPolicyLibrary>();
        }

        [Test]
        public void content_writer_is_registered()
        {
            registeredTypeIs<IContentWriter, ContentWriter>();
        }

        [Test]
        public void the_etag_generator_for_asset_files_is_registered()
        {
            registeredTypeIs<IETagGenerator<IEnumerable<AssetFile>>, AssetFileEtagGenerator>();
        }

        [Test]
        public void asset_content_cache_is_registered()
        {
            new FubuRegistry().BuildGraph().Services.DefaultServiceFor<IAssetContentCache>()
                .Value.ShouldBeOfType<AssetContentCache>();
        }
    }
}