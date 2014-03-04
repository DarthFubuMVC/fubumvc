using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class CacheAttacherTester : InteractionContext<CacheAttacher>
    {
        private List<IFubuSparkView> _views = new List<IFubuSparkView>();
        protected override void beforeEach()
        {
            _views = Services.CreateMockArrayFor<IFubuSparkView>(10).ToList();
            _views.Each(v => v.Stub(x => x.CacheService).PropertyBehavior());
        }

        [Test]
        public void modify_attaches_cache_service_on_views()
        {
            _views.Each(v => ClassUnderTest.Modify(v).As<IFubuSparkView>().CacheService.ShouldEqual(MockFor<ICacheService>()));
        }
    }
}