using System;
using System.Collections.Generic;
using System.IO;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests
{
    [TestFixture]
    public class SparkViewFacilityTester : InteractionContext<SparkViewFacility>
    {
        private IList<SparkItem> _items;

        protected override void beforeEach()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            _items = new List<SparkItem>
            {
                new SparkItem(Path.Combine(root, "Views", "Home", "ModelAView.spark"), root, FubuSparkConstants.HostOrigin) {ViewModelType = typeof (ModelA)},
                new SparkItem(Path.Combine(root, "Views", "Home", "_partial1.spark"), root, FubuSparkConstants.HostOrigin),
                new SparkItem(Path.Combine(root, "Views", "Home", "ModelBView.spark"), root, FubuSparkConstants.HostOrigin) {ViewModelType = typeof (ModelB)},
                new SparkItem(Path.Combine(root, "Views", "Home", "_partial2.spark"), root, FubuSparkConstants.HostOrigin),
                new SparkItem(Path.Combine(root, "Views", "Home", "ModelCView.spark"), root, FubuSparkConstants.HostOrigin) {ViewModelType = typeof (ModelC)},
                new SparkItem(Path.Combine(root, "Views", "Home", "_partial3.spark"), root, FubuSparkConstants.HostOrigin)
            };

            MockFor<ISparkItemComposer>().Expect(c => c.ComposeViews(Arg<TypePool>.Is.Anything)).Return(_items);
        }

        public class ModelA { }
        public class ModelB { }
        public class ModelC { }

        [Test]
        public void find_views_returns_view_tokens_from_items_with_a_view_model_only()
        {
            var views = ClassUnderTest.FindViews(null, null);
            views.ShouldHaveCount(3);
            views.ShouldContain(x => x.ViewModelType == typeof(ModelA));
            views.ShouldContain(x => x.ViewModelType == typeof(ModelB));
            views.ShouldContain(x => x.ViewModelType == typeof(ModelC));
        }

    }
}