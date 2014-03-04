using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class ViewEntryProviderCacheTester : InteractionContext<ViewEntryProviderCache>
    {
        private ISparkViewEngine _engine;
        SparkViewDescriptor _descriptor;
        protected override void beforeEach()
        {
            _engine = MockFor<ISparkViewEngine>();
            _descriptor = new SparkViewDescriptor();
        }

        [Test]
        public void returns_the_entry_from_the_engine()
        {
            var entry = MockFor<ISparkViewEntry>();
            _engine.Stub(x => x.CreateEntry(_descriptor)).Return(entry);
            ClassUnderTest.GetViewEntry(_descriptor).ShouldEqual(entry);
        }

        [Test]
        public void caches_the_entry_from_the_engine()
        {
            var entry = MockRepository.GenerateMock<ISparkViewEntry>();
            entry.Stub(x => x.IsCurrent()).Return(true);
            _engine.Expect(x => x.CreateEntry(_descriptor)).Return(entry).Repeat.Once();
            var result1 = ClassUnderTest.GetViewEntry(_descriptor);
            var result2 = ClassUnderTest.GetViewEntry(_descriptor);

            result1.ShouldEqual(entry).ShouldEqual(result2);
            _engine.VerifyAllExpectations();
        }

        [Test]
        public void if_the_entry_is_not_current_gets_a_new_entry_from_the_engine()
        {
            var entry1 = MockRepository.GenerateMock<ISparkViewEntry>();
            var entry2 = MockRepository.GenerateMock<ISparkViewEntry>();
            entry1.Stub(x => x.IsCurrent()).Return(false);
            entry2.Stub(x => x.IsCurrent()).Return(true);

            _engine.Stub(x => x.CreateEntry(_descriptor)).Return(entry1);
            var result1 = ClassUnderTest.GetViewEntry(_descriptor);

            _engine.BackToRecord();
            _engine.Stub(x => x.CreateEntry(_descriptor)).Return(entry2);
            _engine.Replay();
            var result2 = ClassUnderTest.GetViewEntry(_descriptor);
            var result3 = ClassUnderTest.GetViewEntry(_descriptor);

            result1.ShouldEqual(entry1);
            result2.ShouldEqual(entry2).ShouldEqual(result3);
        }
    }
}