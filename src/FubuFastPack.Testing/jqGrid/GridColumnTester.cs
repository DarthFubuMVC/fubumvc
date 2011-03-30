using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuFastPack.JqGrid;
using FubuFastPack.Testing.Security;
using FubuLocalization;
using FubuMVC.Core.Urls;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace FubuFastPack.Testing.jqGrid
{

    public class StubGridData<T> : IGridData
    {
        private readonly Queue<Cache<Accessor, object>> _rows = new Queue<Cache<Accessor, object>>();
        private Cache<Accessor, object> _currentRow;

        public StubGridData()
        {
            AddRow();
        }

        public void AddRow()
        {
            _currentRow = new Cache<Accessor, object>();
            _rows.Enqueue(_currentRow);
        }

        public void SetValue(Expression<Func<T, object>> property, object value)
        {
            _currentRow[property.ToAccessor()] = value;
        }

        public Func<object> GetterFor(Accessor accessor)
        {
            return () => _currentRow[accessor];
        }

        public bool MoveNext()
        {
            if (!_rows.Any()) return false;

            _currentRow = _rows.Dequeue();
            return true;
        }

        public object CurrentRowType()
        {
            throw new NotImplementedException();
        }
    }

    public class ColumnFillerHarness
    {
        private readonly IDisplayFormatter _formatter = MockRepository.GenerateMock<IDisplayFormatter>();
        private readonly IUrlRegistry _urls = new StubUrlRegistry();

        public IDisplayFormatter Formatter
        {
            get { return _formatter; }
        }

        public IUrlRegistry Urls
        {
            get { return _urls; }
        }

        public EntityDTO RunColumn<T>(IGridColumn column, Action<StubGridData<T>> configure)
        {
            var data = new StubGridData<T>();
            configure(data);

            var filler = column.CreateDtoFiller(data, _formatter, _urls);
            var dto = new EntityDTO();
            filler(dto);

            return dto;
        }
    }

    [TestFixture]
    public class GridColumnTester
    {
        private GridColumn<Case> theColumn;

        [SetUp]
        public void SetUp()
        {
            theColumn = GridColumn<Case>.ColumnFor(x => x.Condition);
        }

        [Test]
        public void the_column_is_sortable_by_default()
        {
            theColumn.IsSortable.ShouldBeTrue();
        }

        [Test]
        public void get_header_with_a_string_token()
        {
            StringToken token = StringToken.FromKeyString("HeaderText");
            theColumn.HeaderFrom(token);

            theColumn.GetHeader().ShouldEqual(token.ToString());
        }

        [Test]
        public void get_headers_returns_the_one_vlaue()
        {
            StringToken token = StringToken.FromKeyString("HeaderText");
            theColumn.HeaderFrom(token);

            theColumn.Headers().Single().ShouldEqual(token.ToString());
        }

        [Test]
        public void get_header_without_a_string_token_should_just_localize_the_header()
        {
            theColumn.GetHeader().ShouldEqual(LocalizationManager.GetHeader<Case>(x => x.Condition));
        }

        [Test]
        public void get_header_by_overriding_the_header_column()
        {
            theColumn.HeaderFrom(c => c.Title);
            theColumn.GetHeader().ShouldEqual(LocalizationManager.GetHeader<Case>(x => x.Title));
        }

        [Test]
        public void if_the_column_is_not_queryable_return_no_filters()
        {
            theColumn.IsFilterable = false;
            theColumn.FilteredProperties().Any().ShouldBeFalse();
        }


        [Test]
        public void return_a_single_select_accessor()
        {
            theColumn.SelectAccessors().Single().Name.ShouldEqual("Condition");
            theColumn.SelectAccessors().Single().Name.ShouldEqual("Condition");
        }

        [Test]
        public void formatter_is_not_assigned_by_default()
        {
            theColumn.ToDictionary().Single().ContainsKey("formatter").ShouldBeFalse();
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

        
    }

    [TestFixture]
    public class when_filling_the_entity_dto_with_the_default_column_settings
    {
        private const string theRawValue = "raw value";
        private const string theFormattedValue = "formatted value";
        private GridColumn<Case> theColumn;
        private EntityDTO theResultingDto;

        [SetUp]
        public void SetUp()
        {
            theColumn = GridColumn<Case>.ColumnFor(x => x.Condition);
            var harness = new ColumnFillerHarness();
            harness.Formatter.Stub(x => x.GetDisplayForValue(theColumn.Accessor, theRawValue)).Return(theFormattedValue);

            theResultingDto = harness.RunColumn<Case>(theColumn, data =>
            {
                data.SetValue(c => c.Condition, theRawValue);
            });
        }

        [Test]
        public void the_value_added_to_the_dto_should_be_the_formatted_value_for_the_raw_accessor_value()
        {
            theResultingDto.cell.Last().ShouldEqual(theFormattedValue);
        }
    }

    [TestFixture]
    public class when_marking_a_column_as_time_ago
    {
        private readonly DateTime theRawValue = new DateTime(2011, 2, 13, 15, 23, 45);
        private readonly string theFormattedValue = "formatted value";
        private GridColumn<Case> theColumn;
        private EntityDTO theResultingDto;
        private IDictionary<string, object> theColModel;

        [SetUp]
        public void SetUp()
        {
            theColumn = GridColumn<Case>.ColumnFor(x => x.Condition).TimeAgo();
            var harness = new ColumnFillerHarness();

            var request = new GetStringRequest(theColumn.Accessor, theRawValue, null){
                Format = "{0:s}"
            };
            harness.Formatter.Stub(x => x.GetDisplay(request)).Return(theFormattedValue);

            theResultingDto = harness.RunColumn<Case>(theColumn, data =>
            {
                data.SetValue(c => c.Condition, theRawValue);
            });

            theColModel = theColumn.ToDictionary().Single();
        }

        [Test]
        public void the_col_model_formatter_should_be_timeAgo()
        {
            theColModel["formatter"].ShouldEqual("timeAgo");
        }


        [Test]
        public void the_value_added_to_the_dto_should_be_the_date_formatted_in_long_time_format()
        {
            theResultingDto.cell.Last().ShouldEqual(theFormattedValue);
        }
    }

}