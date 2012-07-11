using System.Text;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.SlickGrid.Testing
{
    [TestFixture]
    public class ColumnDefinitionTester
    {
        private string writeColumn(IGridColumn<ColumnDefTarget> column)
        {
            var builder = new StringBuilder();
            column.WriteColumn(builder);

            return builder.ToString();
        }

        [Test]
        public void sortable_by_default()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name);
            writeColumn(column).ShouldContain("sortable: true");
        }

        [Test]
        public void override_title()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name);
            column.Title("else");

            writeColumn(column).ShouldContain("name: \"else\"");
        }

        [Test]
        public void override_field()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name);
            column.Field("else");

            writeColumn(column).ShouldEqual("{name: \"Name\", field: \"else\", id: \"Name\", sortable: true}");
        }

        [Test]
        public void override_id()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name);
            column.Id("else");

            writeColumn(column).ShouldEqual("{name: \"Name\", field: \"Name\", id: \"else\", sortable: true}");  
        }

        [Test]
        public void overwrite_sortable()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name).Sortable(false);
            writeColumn(column).ShouldContain("sortable: false"); 
        }

        [Test]
        public void override_resizable()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name).Resizable(false);

            writeColumn(column).ShouldContain("resizable: false");
        }

        [Test]
        public void override_resizable_2()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name).Resizable(true);

            writeColumn(column).ShouldContain("resizable: true");
        }

        [Test]
        public void write_column_basic_with_defaults()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name);

            writeColumn(column).ShouldEqual("{name: \"Name\", field: \"Name\", id: \"Name\", sortable: true}");           
        }

        [Test]
        public void write_column_for_widths()
        {
            var column = new ColumnDefinition<ColumnDefTarget, string>(x => x.Name);
            column.Width(100, 80, 120);

            writeColumn(column).ShouldContain("width: 100, minWidth: 80, maxWidth: 120");
        }
    }

    public class ColumnDefTarget
    {
        public string Name { get; set; }
        public bool IsCool { get; set; }
        public int Count { get; set; }
    }
}