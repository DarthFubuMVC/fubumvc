using System;
using System.Collections.Generic;
using FubuCore.Reflection;
using FubuFastPack.JqGrid;
using FubuFastPack.Testing.Security;
using FubuLocalization;
using FubuMVC.Core.Registration.Routes;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuFastPack.Testing.jqGrid
{
    [TestFixture]
    public class LinkColumnTester
    {
        private LinkColumn<Case> theColumn;

        [SetUp]
        public void SetUp()
        {
            theColumn = LinkColumn<Case>.For(x => x.Condition);
        }

        [Test]
        public void sortable_by_default()
        {
            theColumn.ToDictionary().Single()["sortable"].ShouldEqual(true);
        }

        [Test]
        public void is_filterable_by_default()
        {
            theColumn.IsFilterable.ShouldBeTrue();
        }

        [Test]
        public void should_return_the_display_accessor()
        {
            theColumn.SelectAccessors().Select(x => x.Name).ShouldHaveTheSameElementsAs( "Condition", "Id");
        }

        [Test]
        public void formatter_is_link_by_default()
        {
            theColumn.ToDictionary().Single()["formatter"].ShouldEqual("link");
        }

        [Test]
        public void formatter_can_be_overriden()
        {
            theColumn.Formatter("trimmedLink").ToDictionary().Single()["formatter"].ShouldEqual("trimmedLink");
        }


        [Test]
        public void formatter_can_be_overriden_by_trimmed()
        {
            var dictionary = theColumn.TrimToLength(10).ToDictionary().Single();
            dictionary["formatter"].ShouldEqual("trimmedLink");
            dictionary["trim-length"].ShouldEqual(10);
        }

        [Test]
        public void link_name_is_driven_off_of_the_accessor_name()
        {
            theColumn.ToDictionary().Single()["linkName"].ShouldEqual("linkForCondition");
        }

        [Test]
        public void the_name_in_the_colModel_is_the_accessor_name()
        {
            theColumn.ToDictionary().Single()["name"].ShouldEqual(theColumn.Accessor.Name);
        }

        [Test]
        public void the_index_in_the_colModel_is_the_accessor_name()
        {
            theColumn.ToDictionary().Single()["index"].ShouldEqual(theColumn.Accessor.Name);
        }

        [Test]
        public void sortable_in_the_colModel_when_IsSortable_is_true()
        {
            theColumn.Sortable(true);

            theColumn.ToDictionary().Single()["sortable"].ShouldEqual(true);
        }


        [Test]
        public void sortable_in_the_colModel_when_IsSortable_is_false()
        {
            theColumn.Sortable(false);

            theColumn.ToDictionary().Single()["sortable"].ShouldEqual(false);
        }

        [Test]
        public void returns_a_single_header()
        {
            theColumn.Headers().Single().ShouldEqual(theColumn.GetHeader());
        }

    }

    [TestFixture]
    public class when_writing_data_to_a_normal_link
    {
        private const string theRawValue = "raw value";
        private const string theFormattedValue = "formatted value";
        private LinkColumn<Case> theColumn;
        private ColumnFillerHarness theHarness;
        private EntityDTO theDto;
        private Guid _id = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            theColumn = LinkColumn<Case>.For(x => x.Condition);
            theHarness = new ColumnFillerHarness();
            theHarness.Formatter.Stub(x => x.GetDisplayForValue(theColumn.Accessor, theRawValue)).Return(theFormattedValue);


            theDto = theHarness.RunColumn<Case>(theColumn, x =>
            {
                x.SetValue(c => c.Id, _id);
                x.SetValue(c => c.Condition, theRawValue);
            });
        }

        [Test]
        public void writes_the_formatted_value_to_the_cell_array()
        {
            theDto.cell.Last().ShouldEqual(theFormattedValue);
        }

        [Test]
        public void writes_the_url_to_the_first_column()
        {
            var parameters = new RouteParameters();
            parameters["Id"] = _id.ToString();
            var url = theHarness.Urls.UrlFor(typeof (Case), parameters);
            theDto["linkForCondition"].ShouldEqual(url);
        }


    }
    
    [TestFixture]
    public class when_writing_a_disabled_link_column
    {
        private const string theRawValue = "raw value";
        private const string theFormattedValue = "formatted value";
        private LinkColumn<Case> theColumn;
        private ColumnFillerHarness theHarness;
        private EntityDTO theDto;
        private Guid _id = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            theColumn = LinkColumn<Case>.For(x => x.Condition);
            theColumn.DisableLink();

            theHarness = new ColumnFillerHarness();
            theHarness.Formatter.Stub(x => x.GetDisplayForValue(theColumn.Accessor, theRawValue)).Return(theFormattedValue);


            theDto = theHarness.RunColumn<Case>(theColumn, x =>
            {
                x.SetValue(c => c.Id, _id);
                x.SetValue(c => c.Condition, theRawValue);
            });

            
        }

        [Test]
        public void does_not_write_the_link_to_the_dto()
        {
            theDto.HasProperty("linkForCondition").ShouldBeFalse();
        }

        [Test]
        public void writes_the_formatted_value_to_the_cell_array()
        {
            theDto.cell.Last().ShouldEqual(theFormattedValue);
        }

        [Test]
        public void formatter_is_the_default_empty()
        {
            theColumn.ToDictionary().Single().ContainsKey("formatter").ShouldBeFalse();
        }
    }

    [TestFixture]
    public class when_using_a_command_link
    {
        private CommandColumn<Case, CaseActionModel> theColumn;
        private ColumnFillerHarness theHarness;
        private readonly Guid _id = Guid.NewGuid();
        private EntityDTO theDto;
        private IDictionary<string, object> theColumnModel;

        [SetUp]
        public void SetUp()
        {
            theColumn = new CommandColumn<Case, CaseActionModel>(CommandKey.Action);

            theHarness = new ColumnFillerHarness();


            theDto = theHarness.RunColumn<Case>(theColumn, x =>
            {
                x.SetValue(c => c.Id, _id);
            });

            theColumnModel = theColumn.ToDictionary().Single();
        }

        [Test]
        public void headers_returns_one_header()
        {
            theColumn.Headers().Single().ShouldEqual(theColumn.GetHeader());
        }

        [Test]
        public void needs_to_store_the_link_name_in_the_colModel()
        {
            theColumnModel["linkName"].ShouldEqual("linkForAction");
        }

        [Test]
        public void is_not_sortable()
        {
            theColumnModel["sortable"].ShouldEqual(false);
        }

        [Test]
        public void should_write_the_literal_text_of_the_string_token_to_the_cell()
        {
            theDto.cell.Last().ShouldEqual(CommandKey.Action.ToString());
        }

        [Test]
        public void can_override_the_body()
        {
            theColumn.Body(CommandKey.Install);
            theDto = theHarness.RunColumn<Case>(theColumn, x =>
            {
                x.SetValue(c => c.Id, _id);
            });

            theDto.cell.Last().ShouldEqual(CommandKey.Install.ToString());
        }

        [Test]
        public void the_formatter_in_the_col_model_should_be_command()
        {
            theColumnModel["formatter"].ShouldEqual("command");
        }

        [Test]
        public void header_is_the_stringtoken_key()
        {
            theColumn.GetHeader().ShouldEqual(CommandKey.Action.ToString());
        }

        [Test]
        public void select_accessors_should_include_the_id()
        {
            theColumn.SelectAccessors().Single().ShouldEqual(ReflectionHelper.GetAccessor<Case>(x => x.Id));
        }

        [Test]
        public void all_accessors_should_include_the_id()
        {
            theColumn.AllAccessors().Single().ShouldEqual(ReflectionHelper.GetAccessor<Case>(x => x.Id));
        }

        [Test]
        public void filtered_properties_should_not_have_anything()
        {
            theColumn.FilteredProperties().Any().ShouldBeFalse();
        }

        [Test]
        public void should_write_the_url_to_the_data_column()
        {
            var parameters = new RouteParameters();
            parameters["Id"] = _id.ToString();
            var url = theHarness.Urls.UrlFor(typeof(CaseActionModel), parameters);
            theDto["linkForAction"].ShouldEqual(url);
        }

        [Test]
        public void the_name_and_index_of_the_colModel_should_be_the_key_of_the_string_token()
        {
            theColumnModel["name"].ShouldEqual("Action");
            theColumnModel["index"].ShouldEqual("Action");
        }

        [Test]
        public void can_override_the_formatter()
        {
            theColumn.Formatter("something").ToDictionary().Single()["formatter"].ShouldEqual("something");
        }
    }

    public class CaseActionModel
    {
        public Guid Id { get; set; }
    }

    public class CommandKey : StringToken
    {
        protected CommandKey(string key, string defaultValue) : base(key, defaultValue)
        {
        }

        public static CommandKey Action = new CommandKey("Action", "Action"); 
        public static CommandKey Install = new CommandKey("Install", "Install"); 
    }
}