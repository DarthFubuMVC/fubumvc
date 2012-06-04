using System.Collections.Generic;
using FubuMVC.Diagnostics.SlickGrid;
using NUnit.Framework;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.SlickGrid
{
    [TestFixture]
    public class JsonValueWriterTester
    {
        [Test]
        public void write_string()
        {
            JsonValueWriter.ConvertToJson("name").ShouldEqual("name");
        }

        [Test]
        public void write_int()
        {
            JsonValueWriter.ConvertToJson(1).ShouldEqual(1);
        }

        [Test]
        public void write_null()
        {
            JsonValueWriter.ConvertToJson(null).ShouldBeNull();
        }

        [Test]
        public void write_type()
        {
            var type = GetType();

            var dict = JsonValueWriter.ConvertToJson(type).ShouldBeOfType<IDictionary<string, object>>();
            dict["Name"].ShouldEqual(type.Name);
            dict["FullName"].ShouldEqual(type.FullName);
            dict["Namespace"].ShouldEqual(type.Namespace);
            dict["Assembly"].ShouldEqual(type.Assembly.FullName);
        }
    }
}