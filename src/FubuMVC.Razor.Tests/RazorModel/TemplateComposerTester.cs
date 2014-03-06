using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Razor.Parser.SyntaxTree;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.View.Model;
using FubuMVC.Razor.RazorModel;
using FubuMVC.Razor.Registration;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Razor.Tests.RazorModel
{
    [TestFixture]
    public class TemplateComposerTester : InteractionContext<TemplateComposer<IRazorTemplate>>
    {

        private FakeTemplateBinder _binder1;
        private FakeTemplateBinder _binder2;

        private IRazorTemplate _template1;
        private IRazorTemplate _template2;

        private IList<IRazorTemplate> _policy1Templates;
        private IList<IRazorTemplate> _policy2Templates;

        private IList<IRazorTemplate> _binder1Templates;
        private IList<IRazorTemplate> _binder2Templates;

        private ITemplateRegistry<IRazorTemplate> _templateRegistry;
        private readonly TypePool _types;

        public TemplateComposerTester()
        {
            _types = new TypePool();
            _types.AddAssembly(GetType().Assembly);
        }

        protected override void beforeEach()
        {
            _template1 = new RazorTemplate("tmpl1.cshtml", "x", "o1");
            _template2 = new RazorTemplate("tmpl2.cshtml", "z", "o2");
            _templateRegistry = new TemplateRegistry<IRazorTemplate>(new[] {_template1, _template2});

            var viewParser = MockFor<IViewParser>();
            //null here probably won't work
            viewParser.Stub(x => x.Parse(null)).Return(Enumerable.Empty<Span>());

            Func<string, IViewParser> parser = file => viewParser;
            Services.Inject(parser);
            Services.Inject(_types);
            configurePolicies();
            configureBinders();

            registerBindersAndPolicies();

            OtherTemplateBinder.Reset();
        }

        private void configurePolicies()
        {
            _policy1Templates = new List<IRazorTemplate>();
            _policy2Templates = new List<IRazorTemplate>();
        }

        private void configureBinders()
        {
            _binder1Templates = new List<IRazorTemplate>();
            _binder2Templates = new List<IRazorTemplate>();

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
        }

        [Test]
        public void binders_that_match_are_applied_against_each_razor_item()
        {
            ClassUnderTest.Compose(_templateRegistry);
            _binder1Templates.ShouldHaveCount(1).ShouldContain(_template1);
            _binder2Templates.ShouldHaveCount(1).ShouldContain(_template2);
        }

        [Test]
        public void policies_that_match_are_applied_against_each_razor_item()
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


    }

    public class FakeTemplateBinder : ITemplateBinder<IRazorTemplate>
    {
        public FakeTemplateBinder()
        {
            Filter = new CompositePredicate<IBindRequest<IRazorTemplate>>();
            Action = new CompositeAction<IBindRequest<IRazorTemplate>>();
        }

        public CompositePredicate<IBindRequest<IRazorTemplate>> Filter { get; set; }
        public CompositeAction<IBindRequest<IRazorTemplate>> Action { get; set; }


        public bool CanBind(IBindRequest<IRazorTemplate> request)
        {
            return Filter.MatchesAny(request);
        }

        public void Bind(IBindRequest<IRazorTemplate> request)
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

}