using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.WebForms.Testing
{
    [TestFixture]
    public class when_registering_partial_view_types
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _graph = BehaviorGraph.BuildFrom(registry =>
            {
                registry.RegisterPartials(x => x.For<PartialModel>().Use<PartialView>());
                registry.RegisterPartials(x =>
                {
                    x.For<PartialModel1>().Use<PartialView1>();
                    x.For<PartialModel2>().Use<PartialView2>();
                });
            });

            _partialViewTypeRegistries = _graph.Services.FindAllValues<IPartialViewTypeRegistry>();
            _partialViewTypeRegistries.ShouldHaveCount(1);
        }

        #endregion

        private BehaviorGraph _graph;
        private IEnumerable<IPartialViewTypeRegistry> _partialViewTypeRegistries;

        public class PartialView : FubuPage
        {
        }

        public class PartialModel
        {
        }

        public class PartialView1 : FubuPage
        {
        }

        public class PartialModel1
        {
        }

        public class PartialView2 : FubuPage
        {
        }

        public class PartialModel2
        {
        }

        public class UnregisteredPartialModel
        {
        }

        public class UnregisteredPartialView : FubuPage
        {
        }

        [Test]
        public void registry_should_contain_registered_types()
        {
            _partialViewTypeRegistries.ShouldHave(reg => reg.HasPartialViewTypeFor<PartialModel>() &&
                                                         reg.HasPartialViewTypeFor<PartialModel1>() &&
                                                         reg.HasPartialViewTypeFor<PartialModel2>());
        }

        [Test]
        public void registry_should_not_contain_unregistered_type()
        {
            _partialViewTypeRegistries.ShouldNotHave(reg => reg.HasPartialViewTypeFor<UnregisteredPartialModel>());
        }
    }
}