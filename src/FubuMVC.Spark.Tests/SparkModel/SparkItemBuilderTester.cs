using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark.Compiler;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class SparkItemBuilderTester : InteractionContext<SparkItemBuilder>
    {
        private FakeSparkItemPolicy _policy1;
        private FakeSparkItemPolicy _policy2;

        private FakeSparkItemBinder _binder1;
        private FakeSparkItemBinder _binder2;

        private SparkItem _item1;
        private SparkItem _item2;

        private IList<SparkItem> _policy1Items;
        private IList<SparkItem> _policy2Items;

        private IList<SparkItem> _binder1Items;
        private IList<SparkItem> _binder2Items;

        protected override void beforeEach()
        {
            _item1 = new SparkItem("item1.spark", "x", "o1");
            _item2 = new SparkItem("item2.spark", "z", "o2");

            var chunkLoader = MockFor<IChunkLoader>();
            chunkLoader.Stub(x => x.Load(Arg<SparkItem>.Is.Anything)).Return(Enumerable.Empty<Chunk>());

            Services.InjectArray(new[] { _item1, _item2, });
            Services.Inject(chunkLoader);

            configurePolicies();
            configureBinders();

            registerBindersAndPolicies();
        }

        private void configurePolicies()
        {
            _policy1Items = new List<SparkItem>();
            _policy2Items = new List<SparkItem>();

            _policy1 = new FakeSparkItemPolicy();
            _policy2 = new FakeSparkItemPolicy();

            _policy1.Filter += x => x == _item1;
            _policy2.Filter += x => x == _item2;

            _policy1.Action += _policy1Items.Add;
            _policy2.Action += _policy2Items.Add;
        }

        private void configureBinders()
        {
            _binder1Items = new List<SparkItem>();
            _binder2Items = new List<SparkItem>();

            _binder1 = new FakeSparkItemBinder();
            _binder2 = new FakeSparkItemBinder();

            _binder1.Filter += x => x == _item1;
            _binder2.Filter += x => x == _item2;

            _binder1.Action += _binder1Items.Add;
            _binder2.Action += _binder2Items.Add;
        }

        private void registerBindersAndPolicies()
        {
            ClassUnderTest.AddBinder(_binder1);
            ClassUnderTest.AddBinder(_binder2);

            ClassUnderTest.Apply(_policy1);
            ClassUnderTest.Apply(_policy2);
        }

        [Test]
        public void binders_that_match_are_applied_against_each_spark_item()
        {
            ClassUnderTest.BuildItems();
            _binder1Items.ShouldHaveCount(1).ShouldContain(_item1);
            _binder2Items.ShouldHaveCount(1).ShouldContain(_item2);
        }

        [Test]
        public void policies_that_match_are_applied_against_each_spark_item()
        {
            ClassUnderTest.BuildItems();
            _policy1Items.ShouldHaveCount(1).ShouldContain(_item1);
            _policy2Items.ShouldHaveCount(1).ShouldContain(_item2);
        }
    }

    public class FakeSparkItemBinder : ISparkItemBinder
    {
        public FakeSparkItemBinder()
        {
            Filter = new CompositePredicate<SparkItem>();
            Action = new CompositeAction<SparkItem>();
        }

        public CompositePredicate<SparkItem> Filter { get; set; }
        public CompositeAction<SparkItem> Action { get; set; }


        public bool CanBind(SparkItem item, BindContext context)
        {
            return Filter.MatchesAny(item);
        }

        public void Bind(SparkItem item, BindContext context)
        {
            Action.Do(item);
        }
    }

    public class FakeSparkItemPolicy : ISparkItemPolicy
    {
        public FakeSparkItemPolicy()
        {
            Filter = new CompositePredicate<SparkItem>();
            Action = new CompositeAction<SparkItem>();
        }

        public CompositePredicate<SparkItem> Filter { get; set; }
        public CompositeAction<SparkItem> Action { get; set; }
        public bool Matches(SparkItem item)
        {
            return Filter.MatchesAny(item);
        }

        public void Apply(SparkItem item)
        {
            Action.Do(item);
        }
    }
}