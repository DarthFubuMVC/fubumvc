using System;
using System.Data;
using FubuCore.Binding;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using System.Linq;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class ReaderBinderTester
    {
        private ReaderBinder binder;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var container = new Container(new AppSettingProviderRegistry());

            binder = container.GetInstance<ReaderBinder>();
        }

        #endregion

        [Test]
        public void read_list_of_objects_from_a_data_reader()
        {
            var ds = new DataTable();
            ds.Columns.Add("Name", typeof (string));
            ds.Columns.Add("Age", typeof (int));
            ds.Columns.Add("Day", typeof (DateTime));

            ds.Rows.Add("Jeremy", 36, DateTime.Today);
            ds.Rows.Add("Max", 6, DateTime.Today.AddDays(1));
            ds.Rows.Add("Natalie", 28, DateTime.Today.AddDays(3));

            var reader = new DataTableReader(ds);
            var targets = binder.Build<ReaderTarget>(reader).ToList();

            targets.Count().ShouldEqual(3);

            targets.First().Name.ShouldEqual("Jeremy");
            targets.First().Age.ShouldEqual(36);
            targets.First().Day.ShouldEqual(DateTime.Today);


        }

        [Test]
        public void read_list_of_objects_from_a_data_reader_with_aliases()
        {
            var ds = new DataTable();
            ds.Columns.Add("NotTheName", typeof(string));
            ds.Columns.Add("NotTheAge", typeof(int));
            ds.Columns.Add("Day", typeof(DateTime));

            ds.Rows.Add("Jeremy", 36, DateTime.Today);
            ds.Rows.Add("Max", 6, DateTime.Today.AddDays(1));
            ds.Rows.Add("Natalie", 28, DateTime.Today.AddDays(3));

            binder.SetAlias("Name", "NotTheName");
            binder.SetAlias("Age", "NotTheAge");

            var reader = new DataTableReader(ds);
            var targets = binder.Build<ReaderTarget>(reader).ToList();

            targets.Count().ShouldEqual(3);

            targets.First().Name.ShouldEqual("Jeremy");
            targets.First().Age.ShouldEqual(36);
            targets.First().Day.ShouldEqual(DateTime.Today);
        }
    }

    public class ReaderTarget
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Day { get; set; }
    }
}