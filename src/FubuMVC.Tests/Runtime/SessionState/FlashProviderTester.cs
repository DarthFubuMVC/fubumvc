using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.SessionState;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.SessionState
{
    
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

        [Fact]
        public void should_json_serialize_model()
        {
            _session.Get<string>(FlashProvider.FLASH_KEY).ShouldContain("\"PropInt\":99");
        }

        [Fact]
        public void should_store_model_in_session()
        {
            _session.Get<string>(FlashProvider.FLASH_KEY).ShouldNotBeNull();
        }
    }

    
    public class when_retrieving_empty_flash : the_flash_provider
    {
        [Fact]
        public void should_return_null()
        {
            _flash.Retrieve<TestInputModel>().ShouldBeNull();
        }
    }

    
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

        [Fact]
        public void should_clear_flash_data()
        {
            _flash.Retrieve<TestInputModel>();
            _flash.Retrieve<TestInputModel>().ShouldBeNull();
        }

        [Fact]
        public void should_return_value()
        {
            _flash.Retrieve<TestInputModel>().PropInt.ShouldBe(99);
        }
    }

    public abstract class the_flash_provider
    {
        protected FlashProvider _flash;
        protected BasicSessionState _session;

        protected the_flash_provider()
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