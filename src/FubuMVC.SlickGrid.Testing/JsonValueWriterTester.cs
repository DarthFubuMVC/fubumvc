using System;
using System.Collections.Generic;
using System.Diagnostics;
using HtmlTags;
using NUnit.Framework;
using FubuTestingSupport;
using FubuCore;

namespace FubuMVC.SlickGrid.Testing
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
        public void try_it()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("names", new string[]{"one", "two", "three"});

            Debug.WriteLine(JsonUtil.ToJson(dict));
        }

        [Test]
        public void write_int()
        {
            JsonValueWriter.Clear();
            JsonValueWriter.ConvertToJson(1).ShouldEqual(1);
        }

        [Test]
        public void write_null()
        {
            JsonValueWriter.ConvertToJson(null).ShouldBeNull();
        }

        [Test]
        public void write_date()
        {
            var value = JsonValueWriter.ConvertToJson(DateTime.Now);
        
            Debug.WriteLine((string) value);
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


        [Test]
        public void write_json_for_IMakeMyOwnJson()
        {
            var value = new FakeOwnJsonValue(1, "January");
            JsonValueWriter.ConvertToJson(value)
                .ShouldEqual("1:January");
        }

        [Test]
        public void register_a_policy()
        {
            JsonValueWriter.ConvertToJson(1).ShouldEqual(1);

            JsonValueWriter.RegisterPolicy<int>(i => "*" + i + "*");

            JsonValueWriter.ConvertToJson(1).ShouldEqual("*1*");
        }

        public class FakeOwnJsonValue : IMakeMyOwnJsonValue
        {
            private string _json;

            public FakeOwnJsonValue(int number, string month)
            {
                _json = "{0}:{1}".ToFormat(number, month);
            }

            public object ToJsonValue()
            {
                return _json;
            }
        }
    }
}