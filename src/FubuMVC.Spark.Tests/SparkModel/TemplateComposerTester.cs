using System.Collections.Generic;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Spark.SparkModel;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel
{
    [TestFixture]
    public class TemplateComposerTester : InteractionContext<TemplateComposer>
    {
        private FakeTemplatePolicy _policy1;
        private FakeTemplatePolicy _policy2;

        private FakeTemplateBinder _binder1;
        private FakeTemplateBinder _binder2;

        private ITemplate _template1;
        private ITemplate _template2;

        private IList<ITemplate> _policy1Templates;
        private IList<ITemplate> _policy2Templates;

        private IList<ITemplate> _binder1Templates;
        private IList<ITemplate> _binder2Templates;

        private ITemplateRegistry _templateRegistry;
        private readonly TypePool _types;

        public TemplateComposerTester()
        {
            _types = new TypePool(GetType().Assembly);
        }

        protected override void beforeEach()
        {
            _template1 = new Template("tmpl1.spark", "x", "o1");
            _template2 = new Template("tmpl2.spark", "z", "o2");
            _templateRegistry = new TemplateRegistry {_template1, _template2};

            var parsingRegistrations = MockFor<IParsingRegistrations>();
            parsingRegistrations.Stub(x => x.ParsingFor(Arg<Template>.Is.Anything)).Return(new Parsing());

            Services.Inject(_types);
            configurePolicies();
            configureBinders();

            registerBindersAndPolicies();

            OtherTemplateBinder.Reset();
            OtherTemplatePolicy.Reset();
        }

        private void configurePolicies()
        {
            _policy1Templates = new List<ITemplate>();
            _policy2Templates = new List<ITemplate>();

            _policy1 = new FakeTemplatePolicy();
            _policy2 = new FakeTemplatePolicy();

            _policy1.Filter += x => x == _template1;
            _policy2.Filter += x => x == _template2;

            _policy1.Action += _policy1Templates.Add;
            _policy2.Action += _policy2Templates.Add;
        }

        private void configureBinders()
        {
            _binder1Templates = new List<ITemplate>();
            _binder2Templates = new List<ITemplate>();

            _binder1 = new FakeTemplateBinder();
            _binder2 = new FakeTemplateBinder();

            _binder1.Filter += x => x.Target == _template1;
            _binder2.Filter += x => x.Target == _template2;

            _binder1.Action += x => _binder1Templates.Add(x.Target);
            _binder2.Action += x => _binder2Templates.Add(x.Target);
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
            ClassUnderTest.Compose(_templateRegistry);
            _binder1Templates.ShouldHaveCount(1).ShouldContain(_template1);
            _binder2Templates.ShouldHaveCount(1).ShouldContain(_template2);
        }

        [Test]
        public void policies_that_match_are_applied_against_each_spark_item()
        {
            ClassUnderTest.Compose(_templateRegistry);
            _policy1Templates.ShouldHaveCount(1).ShouldContain(_template1);
            _policy2Templates.ShouldHaveCount(1).ShouldContain(_template2);
        }

        [Test]
        public void add_binder_register_the_binders_and_use_them_when_building_items()
        {
            var invoked = false;

            var binder = new FakeTemplateBinder();
            binder.Action += x => invoked = true;

            ClassUnderTest
                .AddBinder(binder)
                .AddBinder<OtherTemplateBinder>();

            ClassUnderTest.Compose(_templateRegistry);

            invoked.ShouldBeTrue();
            OtherTemplateBinder.Invoked.ShouldBeTrue();
        }

        [Test]
        public void apply_register_the_policies_and_use_them_when_building_items()
        {
            var invoked1 = false;
            var invoked2 = false;

            var policy = new FakeTemplatePolicy();
            policy.Action += x => invoked1 = true;

            ClassUnderTest.Apply(policy);
            ClassUnderTest.Apply<FakeTemplatePolicy>(p => p.Action += x => invoked2 = true);
            ClassUnderTest.Apply<OtherTemplatePolicy>();

            ClassUnderTest.Compose(_templateRegistry);

            invoked1.ShouldBeTrue();
            invoked2.ShouldBeTrue();
            OtherTemplatePolicy.Invoked.ShouldBeTrue();
        }
    }

    public class FakeTemplateBinder : ITemplateBinder
    {
        public FakeTemplateBinder()
        {
            Filter = new CompositePredicate<IBindRequest>();
            Action = new CompositeAction<IBindRequest>();
        }

        public CompositePredicate<IBindRequest> Filter { get; set; }
        public CompositeAction<IBindRequest> Action { get; set; }


        public bool CanBind(IBindRequest request)
        {
            return Filter.MatchesAny(request);
        }

        public void Bind(IBindRequest request)
        {
            Action.Do(request);
        }
    }

    public class OtherTemplateBinder : FakeTemplateBinder
    {
        public OtherTemplateBinder()
        {
            Action += x => Invoked = true;
        }
        public static void Reset()
        {
            Invoked = false;
        }
        public static bool Invoked { get; private set; }
    }

    public class FakeTemplatePolicy : ITemplatePolicy
    {
        public FakeTemplatePolicy()
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

    public class OtherTemplatePolicy : FakeTemplatePolicy
    {
        public OtherTemplatePolicy()
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