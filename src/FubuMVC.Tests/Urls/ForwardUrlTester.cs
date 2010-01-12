using FubuMVC.Core.Urls;
using NUnit.Framework;

namespace FubuMVC.Tests.Urls
{
    [TestFixture]
    public class ForwardUrlTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            url = new ForwardUrl(typeof (InputModel), m => "something", Categories.NEW, "something");
        }

        #endregion

        private ForwardUrl url;

        public class InputModel
        {
        }

        [Test]
        public void category()
        {
            url.Category.ShouldEqual(Categories.NEW);
        }

        [Test]
        public void get_url()
        {
            url.CreateUrl(new InputModel()).ShouldEqual("something");
        }

        [Test]
        public void input_type_matches()
        {
            url.Category.ShouldEqual(Categories.NEW);
        }
    }
}