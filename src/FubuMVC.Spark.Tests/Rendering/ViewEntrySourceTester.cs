using System.Collections.Generic;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ViewEntrySourceTester : InteractionContext<ViewEntrySource>
    {
        private IDictionary<int, ISparkViewEntry> _cache;
        private SparkViewDescriptor _descriptor;
        private ISparkViewEngine _engine;

        protected override void beforeEach()
        {
            _cache = new Dictionary<int, ISparkViewEntry>();
            _descriptor = new SparkViewDescriptor();
            _descriptor.AddTemplate("Views/Home/Home.spark");
            _engine = MockFor<ISparkViewEngine>();

            Services.Inject(_cache);
            Services.Inject(_descriptor);
        }

        [Test]
        public void if_entry_exists_in_cache_and_is_current_returns_entry()
        {
            var entry = MockFor<ISparkViewEntry>();
            entry.Stub(x => x.IsCurrent()).Return(true);
            _cache[_descriptor.GetHashCode()] = entry;
            ClassUnderTest.GetViewEntry().ShouldEqual(entry);
        }

        [Test]
        public void if_entry_not_exists_creates_the_entry_using_the_engine()
        {
            var entry = MockFor<ISparkViewEntry>();
            _engine.Stub(x => x.CreateEntry(_descriptor)).Return(entry);
            ClassUnderTest.GetViewEntry().ShouldEqual(entry);
            _cache[_descriptor.GetHashCode()].ShouldEqual(entry);
        }

        [Test]
        public void if_entry_exists_but_is_null_creates_the_entry_using_the_engine()
        {
            var entry = MockFor<ISparkViewEntry>();
            _cache[_descriptor.GetHashCode()] = null;
            _engine.Stub(x => x.CreateEntry(_descriptor)).Return(entry);
            ClassUnderTest.GetViewEntry().ShouldEqual(entry);
            _cache[_descriptor.GetHashCode()].ShouldEqual(entry);
        }

        [Test]
        public void if_entry_exists_but_is_not_current_creates_the_entry_using_the_engine()
        {
            var oldEntry = MockFor<ISparkViewEntry>();
            var newEntry = MockFor<ISparkViewEntry>();

            _engine.Stub(x => x.CreateEntry(_descriptor)).Return(newEntry);
            oldEntry.Stub(x => x.IsCurrent()).Return(false);
            _cache[_descriptor.GetHashCode()] = oldEntry;
            ClassUnderTest.GetViewEntry().ShouldEqual(newEntry);
            _cache[_descriptor.GetHashCode()].ShouldEqual(newEntry);
        }
    }
}