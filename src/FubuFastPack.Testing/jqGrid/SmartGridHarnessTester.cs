using System;
using FubuCore.Binding;
using FubuFastPack.JqGrid;
using FubuMVC.Core.Urls;
using FubuMVC.Tests;
using FubuTestApplication.Domain;
using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class smart_grid_harness_with_no_grid_args_ : InteractionContext<SmartGridHarness<NoArgGrid>>
    {
        private StubUrlRegistry urls;

        protected override void beforeEach()
        {
            urls = new StubUrlRegistry();
            Services.Inject<IUrlRegistry>(urls);
        }

        [Test]
        public void url_on_the_model_has_no_query_string()
        {
            var url = urls.UrlFor(new GridRequest<NoArgGrid>());
            ClassUnderTest.BuildModel().url.ShouldEqual(url);
        }

        [Test]
        public void throw_smartgridexception_when_trying_to_register_an_argument_that_is_not_valid()
        {
            Exception<SmartGridException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.RegisterArgument("anything", "else");
            });
            
        }

    }

    [TestFixture]
    public class smart_grid_harness_with_a_single_entity_arg : InteractionContext<SmartGridHarness<EntityArgGrid>>
    {
        private StubUrlRegistry urls;

        protected override void beforeEach()
        {
            urls = new StubUrlRegistry();
            Services.Inject<IUrlRegistry>(urls);
        }

        [Test]
        public void url_on_the_model_has_a_query_string_for_the_person_arg()
        {
            var person = new Person{
                Id = Guid.NewGuid()
            };

            MockFor<ISmartRequest>().Stub(x => x.Value(typeof (Person), "person")).Return(person);

            var url = urls.UrlFor(new GridRequest<EntityArgGrid>());
            url += "?person=" + person.Id.ToString();

            ClassUnderTest.GetUrl().ShouldEqual(url);
        }

        [Test]
        public void build_the_grids_with_the_proper_arguments()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };

            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(Person), "person")).Return(person);

            ClassUnderTest.BuildGrid().Person.ShouldBeTheSameAs(person);
        }

        [Test]
        public void throw_smart_grid_exception_if_argument_is_not_right_type()
        {
            Exception<SmartGridException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.RegisterArgument("person", "wrong type");
            });
            
        }
    }

    [TestFixture]
    public class smart_grid_harness_with_a_single_simple_arg : InteractionContext<SmartGridHarness<StringArgGrid>>
    {
        private StubUrlRegistry urls;

        protected override void beforeEach()
        {
            urls = new StubUrlRegistry();
            Services.Inject<IUrlRegistry>(urls);
        }

        [Test]
        public void url_on_the_model_has_a_query_string_for_the_title_arg()
        {
            var theTitle = "something";
            MockFor<ISmartRequest>().Stub(x => x.Value(typeof (string), "title")).Return(theTitle);

            var url = urls.UrlFor(new GridRequest<StringArgGrid>());
            url += "?title=" + theTitle;

            ClassUnderTest.GetUrl().ShouldEqual(url);
        }


        [Test]
        public void url_on_the_model_has_a_query_string_for_the_title_arg_is_url_encoded()
        {
            var theTitle = "something else";
            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(string), "title")).Return(theTitle);

            var url = urls.UrlFor(new GridRequest<StringArgGrid>());
            url += "?title=" + theTitle.UrlEncoded();

            ClassUnderTest.GetUrl().ShouldEqual(url);
        }

        [Test]
        public void build_the_grid_with_the_proper_arguments()
        {
            var theTitle = "something";
            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(string), "title")).Return(theTitle);

            ClassUnderTest.BuildGrid().Title.ShouldEqual(theTitle);
        }
    }

    [TestFixture]
    public class smart_grid_harness_for_a_grid_with_multiple_args : InteractionContext<SmartGridHarness<MultiArgGrid>>
    {
        private StubUrlRegistry urls;

        protected override void beforeEach()
        {
            urls = new StubUrlRegistry();
            Services.Inject<IUrlRegistry>(urls);
        }

        [Test]
        public void url_on_the_model_has_a_query_string_for_both_the_person_and_the_title_arg()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };

            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(Person), "person")).Return(person);


            var theTitle = "something";
            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(string), "title")).Return(theTitle);

            var url = urls.UrlFor(new GridRequest<MultiArgGrid>());
            url += "?person=" + person.Id.ToString();
            url += "&title=" + theTitle;

            ClassUnderTest.GetUrl().ShouldEqual(url);
        }

        [Test]
        public void build_the_grid_with_the_proper_arguments()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };

            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(Person), "person")).Return(person);


            var theTitle = "something";
            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(string), "title")).Return(theTitle);


            var grid = ClassUnderTest.BuildGrid();

            grid.Title.ShouldEqual(theTitle);
            grid.Person.ShouldBeTheSameAs(person);
        }

    }

    public class NoArgGrid : ProjectionGrid<Case>
    {
        
    }

    public class EntityArgGrid : ProjectionGrid<Case>
    {
        private readonly Person _person;

        public EntityArgGrid(Person person)
        {
            _person = person;
        }

        public Person Person
        {
            get { return _person; }
        }
    }

    public class StringArgGrid : ProjectionGrid<Case>
    {
        private readonly string _title;

        public StringArgGrid(string title)
        {
            _title = title;
        }

        public string Title
        {
            get { return _title; }
        }
    }

    public class MultiArgGrid : ProjectionGrid<Case>
    {
        private readonly Person _person;
        private readonly string _title;

        public MultiArgGrid(Person person, string title)
        {
            _person = person;
            _title = title;
        }

        public Person Person
        {
            get { return _person; }
        }

        public string Title
        {
            get { return _title; }
        }
    }
}