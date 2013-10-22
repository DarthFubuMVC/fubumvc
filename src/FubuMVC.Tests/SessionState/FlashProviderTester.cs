using FubuMVC.Core.Runtime;
using FubuMVC.Core.SessionState;
using FubuTestingSupport;
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
            _session.Get<string>(FlashProvider.FLASH_KEY).ShouldContain("\"PropInt\":99");
        }

        [Test]
        public void should_store_model_in_session()
        {
            _session.Get<string>(FlashProvider.FLASH_KEY).ShouldNotBeNull();
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
        protected BasicSessionState _session;

        [SetUp]
        public void SetUp()
        {
            _session = new BasicSessionState();
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
}