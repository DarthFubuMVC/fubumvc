using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using FubuCore;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Readers;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class FormRequestReaderTester
    {
        [Test]
        public void can_parse_query_string_with_encoding()
        {
            runFormReader("Anesth=Moore%2C+Roy")["Anesth"].ShouldEqual("Moore, Roy");
        }

        [Test]
        public void can_parse_field_values_in_query_string()
        {
            runFormReader("Moore%2C+Roy=Anesth")["Moore, Roy"].ShouldEqual("Anesth");
        }

        [Test]
        public void can_parse_multiple_values()
        {
            var dict = runFormReader("a=1&b=2&c=3");

            dict["a"].ShouldEqual("1");
            dict["b"].ShouldEqual("2");
            dict["c"].ShouldEqual("3");

            dict.Count.ShouldEqual(3);
        }

        private NameValueCollection runFormReader(string formPost)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(formPost);
            var stream = new MemoryStream(bytes);
            var environment = new Dictionary<string, object>();
            environment.Add(OwinConstants.MediaTypeKey, MimeType.HttpFormMimetype);
            environment.Add(OwinConstants.RequestBodyKey, stream);
            new FormReader().Read(environment);
            var form = environment.Get<NameValueCollection>(OwinConstants.RequestFormKey);
            return form;
        }
    }
}