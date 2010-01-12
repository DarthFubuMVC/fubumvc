using System.Collections.Generic;
using System.Web;
using FubuMVC.Core.SessionState;
using NUnit.Framework;

namespace FubuMVC.Tests.SessionState
{
    [TestFixture]
    public class when_storing_flash : the_flash_provider
    {
        protected override void beforeEach()
        {
            var model = new TestInputModel
            {
                PropInt = 99
            };
            _flash.Flash(model);
        }

        [Test]
        public void should_json_serialize_model()
        {
            _session.Values[FlashProvider.FLASH_KEY].ShouldBeOfType<string>().ShouldContain("\"PropInt\":99");
        }

        [Test]
        public void should_store_model_in_session()
        {
            _session.Values[FlashProvider.FLASH_KEY].ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class when_retrieving_empty_flash : the_flash_provider
    {
        [Test]
        public void should_return_null()
        {
            _flash.Retrieve<TestInputModel>().ShouldBeNull();
        }
    }

    [TestFixture]
    public class when_retrieving_flash_data : the_flash_provider
    {
        protected override void beforeEach()
        {
            var model = new TestInputModel
            {
                PropInt = 99
            };
            _flash.Flash(model);
        }

        [Test]
        public void should_clear_flash_data()
        {
            _flash.Retrieve<TestInputModel>();
            _flash.Retrieve<TestInputModel>().ShouldBeNull();
        }

        [Test]
        public void should_return_value()
        {
            _flash.Retrieve<TestInputModel>().PropInt.ShouldEqual(99);
        }
    }

    public abstract class the_flash_provider
    {
        protected FlashProvider _flash;
        protected SessionMock _session;

        [SetUp]
        public void SetUp()
        {
            _session = new SessionMock();
            _flash = new FlashProvider
            {
                Session = _session
            };
            beforeEach();
        }

        protected virtual void beforeEach()
        {
        }
    }

    public class SessionMock : HttpSessionStateBase
    {
        private readonly IDictionary<string, object> _values = new Dictionary<string, object>();
        public IDictionary<string, object> Values { get { return _values; } }

        public override object this[string name] { get { return Values.ContainsKey(name) ? Values[name] : null; } set { Values[name] = value; } }

        public override void Remove(string name)
        {
            Values.Remove(name);
        }
    }
}