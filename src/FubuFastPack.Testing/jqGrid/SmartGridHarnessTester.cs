using System;
using FubuCore.Binding;
using FubuCore.Reflection;
using FubuFastPack.JqGrid;
using FubuFastPack.Querying;
using FubuLocalization;
using FubuMVC.Core.Urls;
using FubuMVC.Tests;
using FubuTestApplication.Domain;
using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;
using System.Linq;

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
        public void string_arguments_should_be_empty()
        {
            ClassUnderTest.GetArgumentsAsString().Any().ShouldBeFalse();
        }

        [Test]
        public void url_on_the_model_has_no_query_string()
        {
            var url = urls.UrlFor(new GridRequest<NoArgGrid>());
            ClassUnderTest.BuildJqModel().url.ShouldEqual(url);
        }

        [Test]
        public void throw_smartgridexception_when_trying_to_register_an_argument_that_is_not_valid()
        {
            Exception<SmartGridException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.RegisterArgument("anything", "else");
            });
            
        }

        [Test]
        public void grid_type()
        {
            ClassUnderTest.GridType.ShouldEqual(typeof (NoArgGrid));
        }

        [Test]
        public void trying_to_pass_in_arguments_throws_the_smart_grid_exception()
        {
            Exception<SmartGridException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.RegisterArguments("a");
            });
        }
    }

    [TestFixture]
    public class when_building_the_grid_model
    {
        private StubUrlRegistry urls;
        private InMemorySmartRequest request;
        private QueryService queryService;

        [SetUp]
        public void SetUp()
        {
            urls = new StubUrlRegistry();
            request = new InMemorySmartRequest();
            queryService = new QueryService(new IFilterHandler[0]);
        }


        private ISmartGridHarness harnessFor<T>() where T : ISmartGrid
        {
            return new SmartGridHarness<T>(null, urls, queryService, request);
        }

        [Test]
        public void allow_create_new_negative()
        {
            harnessFor<SimpleGrid>().BuildGridModel().AllowCreateNew.ShouldBeFalse();
        }

        [Test]
        public void allow_create_new_positive()
        {
            harnessFor<CanCreateNewGrid>().BuildGridModel().AllowCreateNew.ShouldBeTrue();
        }

        [Test]
        public void can_save_query_negative()
        {
            harnessFor<SimpleGrid>().BuildGridModel().CanSaveQuery.ShouldBeFalse();
        }

        [Test]
        public void can_save_query_positive()
        {
            harnessFor<CanCreateNewGrid>().BuildGridModel().CanSaveQuery.ShouldBeTrue();
        }

        [Test]
        public void get_filtered_properties()
        {
            var filters = harnessFor<FilteredGrid>().BuildGridModel().FilteredProperties;
            filters.Any(x => x.Accessor.Name == "CaseType").ShouldBeTrue();
            filters.Any(x => x.Accessor.Name == "Condition").ShouldBeTrue();
        }

        [Test]
        public void jq_grid_model_is_set()
        {
            harnessFor<SimpleGrid>().BuildGridModel().GridModel.ShouldNotBeNull();
        }

        [Test]
        public void grid_name_is_set_on_build_model()
        {
            harnessFor<SimpleGrid>().BuildGridModel().GridName.ShouldEqual("Simple");
        }

        [Test]
        public void grid_type_is_set_on_build_model()
        {
            harnessFor<SimpleGrid>().BuildGridModel().GridType.ShouldEqual(typeof (SimpleGrid));
        }

        [Test]
        public void header_text_is_pulled_from_grid()
        {
            harnessFor<SimpleGrid>().BuildGridModel().HeaderText.ShouldEqual("The simple grid");
        }

        [Test]
        public void allow_creation_is_false_so_no_new_entity_values()
        {
            var model = harnessFor<SimpleGrid>().BuildGridModel();
            model.NewEntityText.ShouldBeNull();
            model.NewEntityUrl.ShouldBeNull();
        }

        [Test]
        public void allow_creation_is_true_should_get_new_entity_values_on_grid_model()
        {
            var model = harnessFor<CanCreateNewGrid>().BuildGridModel();
            model.NewEntityText.ShouldEqual(StringToken.FromKeyString("CREATE_NEW_" + typeof(Case)).ToString());
            model.NewEntityUrl.ShouldEqual(urls.UrlForNew(typeof (Case)));
        }

        [Test]
        public void puts_all_the_initial_criteria_into_the_grid_model()
        {
            var model = harnessFor<CriteriaGrid>().BuildGridModel();
            model.InitialCriteria().Count().ShouldEqual(2);
            model.InitialCriteria().Any(x => x.property == "CaseType").ShouldBeTrue();
        }
    }


    public class CriteriaGrid : ProjectionGrid<Case>
    {
        public CriteriaGrid()
        {
            AddCriteria(c => c.CaseType, OperatorKeys.EQUAL, "Question");
            AddCriteria(c => c.Condition, OperatorKeys.EQUAL, "Open");
        }
    }

    public class FilteredGrid : ProjectionGrid<Case>
    {
        public FilteredGrid()
        {
            FilterOn(x => x.CaseType);
            FilterOn(x => x.Condition);
        }
    }

    public class SimpleGrid : ProjectionGrid<Case>
    {
        public SimpleGrid()
        {
            Show(x => x.Title);
            CanSaveQuery(false);
        }

        public override string GetHeader()
        {
            return "The simple grid";
        }
    }

    public class CanCreateNewGrid : ProjectionGrid<Case>
    {
        public CanCreateNewGrid()
        {
            AllowCreateNew();
            CanSaveQuery(true);
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
        public void get_arguments_as_string_should_convert_entity_to_the_id_string()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };
            ClassUnderTest.RegisterArgument("person", person);

            ClassUnderTest.GetArgumentsAsString().Single().ShouldEqual(person.Id.ToString());
        }

        [Test]
        public void get_header_text_delegates_to_the_grid()
        {
            ClassUnderTest.HeaderText().ShouldEqual(new EntityArgGrid(null).GetHeader());
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
        public void get_url_query_string()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };

            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(Person), "person")).Return(person);

            ClassUnderTest.GetQuerystring().ShouldEqual("?person=" + person.Id.ToString());
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
        public void get_arguments_as_string_array()
        {
            ClassUnderTest.RegisterArgument("title", "the title");

            ClassUnderTest.GetArgumentsAsString().Single().ShouldEqual("the title");
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
        public void get_arguments_as_string_array()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };

            ClassUnderTest.RegisterArguments(person, "the title");

            ClassUnderTest.GetArgumentsAsString().ShouldHaveTheSameElementsAs(person.Id.ToString(), "the title");
        }

        [Test]
        public void get_query_string()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };

            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(Person), "person")).Return(person);


            var theTitle = "something";
            MockFor<ISmartRequest>().Stub(x => x.Value(typeof(string), "title")).Return(theTitle);

            var expected = "?person=" + person.Id.ToString();
            expected += "&title=" + theTitle;

            ClassUnderTest.GetQuerystring().ShouldEqual(expected);
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

        [Test]
        public void register_arguments_happy_path()
        {
            var person = new Person
            {
                Id = Guid.NewGuid()
            };

            var theTitle = "something";

            ClassUnderTest.RegisterArguments(person, theTitle);
            var grid = ClassUnderTest.BuildGrid();

            grid.Title.ShouldEqual(theTitle);
            grid.Person.ShouldBeTheSameAs(person);
        }

        [Test]
        public void register_arguments_with_the_correct_number_but_a_wrong_type()
        {
            Exception<SmartGridException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.RegisterArguments("not a person", "a perfectly good title");
            });
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

        public override string GetHeader()
        {
            return "header of the entity arg grid";
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