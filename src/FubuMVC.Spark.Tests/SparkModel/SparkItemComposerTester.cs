using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using Spark.Compiler;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class SparkItemComposerTester : InteractionContext<SparkItemComposer>
    {
        private FakeSparkTemplatePolicy _policy1;
        private FakeSparkTemplatePolicy _policy2;

        private FakeSparkTemplateBinder _binder1;
        private FakeSparkTemplateBinder _binder2;

        private ITemplate _item1;
        private ITemplate _item2;

        private IList<ITemplate> _policy1Items;
        private IList<ITemplate> _policy2Items;

        private IList<ITemplate> _binder1Items;
        private IList<ITemplate> _binder2Items;

        private readonly TypePool _typePool;

        public SparkItemComposerTester()
        {
            _typePool = new TypePool(GetType().Assembly);
        }

        protected override void beforeEach()
        {
            _item1 = new SparkItem("item1.spark", "x", "o1");
            _item2 = new SparkItem("item2.spark", "z", "o2");

            var chunkLoader = MockFor<IChunkLoader>();
            chunkLoader.Stub(x => x.Load(Arg<SparkItem>.Is.Anything)).Return(Enumerable.Empty<Chunk>());

            Services.InjectArray(new[] { _item1, _item2 });
            Services.Inject(chunkLoader);

            configurePolicies();
            configureBinders();

            registerBindersAndPolicies();

            OtherSparkTemplateBinder.Reset();
            OtherSparkTemplatePolicy.Reset();
        }

        private void configurePolicies()
        {
            _policy1Items = new List<ITemplate>();
            _policy2Items = new List<ITemplate>();

            _policy1 = new FakeSparkTemplatePolicy();
            _policy2 = new FakeSparkTemplatePolicy();

            _policy1.Filter += x => x == _item1;
            _policy2.Filter += x => x == _item2;

            _policy1.Action += _policy1Items.Add;
            _policy2.Action += _policy2Items.Add;
        }

        private void configureBinders()
        {
            _binder1Items = new List<ITemplate>();
            _binder2Items = new List<ITemplate>();

            _binder1 = new FakeSparkTemplateBinder();
            _binder2 = new FakeSparkTemplateBinder();

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
            ClassUnderTest.ComposeViews(_typePool);
            _binder1Items.ShouldHaveCount(1).ShouldContain(_item1);
            _binder2Items.ShouldHaveCount(1).ShouldContain(_item2);
        }

        [Test]
        public void policies_that_match_are_applied_against_each_spark_item()
        {
            ClassUnderTest.ComposeViews(_typePool);
            _policy1Items.ShouldHaveCount(1).ShouldContain(_item1);
            _policy2Items.ShouldHaveCount(1).ShouldContain(_item2);
        }

        [Test]
        public void add_binder_register_the_binders_and_use_them_when_building_items()
        {
            var invoked = false;

            var binder = new FakeSparkTemplateBinder();
            binder.Action += x => invoked = true;

            ClassUnderTest
                .AddBinder(binder)
                .AddBinder<OtherSparkTemplateBinder>();

            ClassUnderTest.ComposeViews(_typePool);

            invoked.ShouldBeTrue();
            OtherSparkTemplateBinder.Invoked.ShouldBeTrue();
        }

        [Test]
        public void apply_register_the_policies_and_use_them_when_building_items()
        {
            var invoked1 = false;
            var invoked2 = false;

            var policy = new FakeSparkTemplatePolicy();
            policy.Action += x => invoked1 = true;

            ClassUnderTest.Apply(policy);
            ClassUnderTest.Apply<FakeSparkTemplatePolicy>(p => p.Action += x => invoked2 = true);
            ClassUnderTest.Apply<OtherSparkTemplatePolicy>();

            ClassUnderTest.ComposeViews(_typePool);

            invoked1.ShouldBeTrue();
            invoked2.ShouldBeTrue();
            OtherSparkTemplatePolicy.Invoked.ShouldBeTrue();
        }
    }

    public class FakeSparkTemplateBinder : ISparkTemplateBinder
    {
        public FakeSparkTemplateBinder()
        {
            Filter = new CompositePredicate<ITemplate>();
            Action = new CompositeAction<ITemplate>();
        }

        public CompositePredicate<ITemplate> Filter { get; set; }
        public CompositeAction<ITemplate> Action { get; set; }


        public bool CanBind(ITemplate template, BindContext context)
        {
            return Filter.MatchesAny(template);
        }

        public void Bind(ITemplate template, BindContext context)
        {
            Action.Do(template);
        }
    }

    public class OtherSparkTemplateBinder : FakeSparkTemplateBinder
    {
        public OtherSparkTemplateBinder()
        {
            Action += x => Invoked = true;
        }
        public static void Reset()
        {
            Invoked = false;
        }
        public static bool Invoked { get; private set; }
    }

    public class FakeSparkTemplatePolicy : ISparkTemplatePolicy
    {
        public FakeSparkTemplatePolicy()
        {
            Filter = new CompositePredicate<ITemplate>();
            Action = new CompositeAction<ITemplate>();
        }

        public CompositePredicate<ITemplate> Filter { get; set; }
        public CompositeAction<ITemplate> Action { get; set; }
        public bool Matches(ITemplate template)
        {
            return Filter.MatchesAny(template);
        }

        public void Apply(ITemplate template)
        {
            Action.Do(template);
        }
    }

    public class OtherSparkTemplatePolicy : FakeSparkTemplatePolicy
    {
        public OtherSparkTemplatePolicy()
        {
            Action += x => Invoked = true;
        }
        public static void Reset()
        {
            Invoked = false;
        }
        public static bool Invoked { get; private set; }
    }
}