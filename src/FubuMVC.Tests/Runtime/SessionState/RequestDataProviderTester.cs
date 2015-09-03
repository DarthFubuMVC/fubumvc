using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using FubuMVC.Core.Runtime.SessionState;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.SessionState
{
    [TestFixture]
    public class RequestDataProviderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup_Test()
        {
            _httpContext = MockRepository.GenerateMock<HttpContextBase>();
            _sessionState = MockRepository.GenerateMock<HttpSessionStateBase>();

            _flashViewModelForTesting = new FlashViewModelForTesting
            {
                Property1 = "Property",
                Property2 = 10,
                Property3 = true,
            };
        }

        #endregion

        private HttpContextBase _httpContext;
        private HttpSessionStateBase _sessionState;
        private RequestDataProvider _requestDataProvider;
        private FlashViewModelForTesting _flashViewModelForTesting;

        [Test]
        public void Should_be_able_to_load_model_when_Session_is_not_null()
        {
            _httpContext.Stub(x => x.Session).Return(_sessionState);
            _sessionState.Expect(c => c[RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property1"]).Return(
                _flashViewModelForTesting.Property1);
            _sessionState.Expect(c => c[RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property2"]).Return(
                _flashViewModelForTesting.Property2);
            _sessionState.Expect(c => c[RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property3"]).Return(
                _flashViewModelForTesting.Property3);

            var testing = new RequestDataProvider(_httpContext).Load<FlashViewModelForTesting>();

            _httpContext.VerifyAllExpectations();
            _sessionState.VerifyAllExpectations();

            testing.Property1.ShouldBe(_flashViewModelForTesting.Property1);
            testing.Property2.ShouldBe(_flashViewModelForTesting.Property2);
            testing.Property3.ShouldBe(_flashViewModelForTesting.Property3);
        }

        [Test]
        public void Should_be_able_to_store_model_when_Session_is_not_null()
        {
            _httpContext.Stub(x => x.Session).Return(_sessionState);
            _sessionState.Expect(
                c =>
                c.Add(RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property1", _flashViewModelForTesting.Property1));
            _sessionState.Expect(
                c =>
                c.Add(RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property2", _flashViewModelForTesting.Property2));
            _sessionState.Expect(
                c =>
                c.Add(RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property3", _flashViewModelForTesting.Property3));

            new RequestDataProvider(_httpContext).Store(_flashViewModelForTesting);

            _httpContext.VerifyAllExpectations();
            _sessionState.VerifyAllExpectations();
        }

        [Test]
        public void Should_do_nothing_when_trying_to_set_the_SessionState_when_the_current_session_is_null()
        {
            _httpContext.Stub(x => x.Session).Return(null);

            _requestDataProvider = new RequestDataProvider(_httpContext);

            _requestDataProvider.Store(_flashViewModelForTesting);

            _requestDataProvider.Load<FlashViewModelForTesting>().ShouldBeNull();

            _httpContext.VerifyAllExpectations();
        }

        [Test]
        public void Should_only_clear_all_flash_related_session_data()
        {
            _httpContext.Stub(x => x.Session).Return(_sessionState);
            var nameObjectCollection = new NameObjectCollection
            {
                {RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property1", _flashViewModelForTesting.Property1},
                {"OtherData", new object()},
                {RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property2", _flashViewModelForTesting.Property3}
            };
            _sessionState.Stub(x => x.Keys).Return(nameObjectCollection.Keys);

            new RequestDataProvider(_httpContext).Clear();

            _sessionState.AssertWasCalled(x => x.Remove(RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property1"));
            _sessionState.AssertWasNotCalled(x => x.Remove("OtherData"));
            _sessionState.AssertWasCalled(x => x.Remove(RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property2"));
        }

        [Test]
        public void Should_return_should_throw_when_a_property_was_not_found_in_the_session()
        {
            Exception<KeyNotFoundException>.ShouldBeThrownBy(() =>
            {
                _httpContext.Stub(x => x.Session).Return(_sessionState);
                _sessionState.Expect(c => c[RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property1"]).Return(
                    _flashViewModelForTesting.Property1);

                // Note kept here to show the difference this is commented out as part of the test
                //_sessionState.Expect(c => c[RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property2"]).Return(_flashViewModelForTesting.Property2);

                _sessionState.Expect(c => c[RequestDataProvider.REQUESTDATA_PREFIX_KEY + "Property3"]).Return(
                    _flashViewModelForTesting.Property3);

                new RequestDataProvider(_httpContext).Load<FlashViewModelForTesting>();
            });


        }
    }

    public class FlashViewModelForTesting
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }
        public bool Property3 { get; set; }
    }

    public class NameObjectCollection : NameObjectCollectionBase
    {
        public void Add(String key, Object value)
        {
            BaseAdd(key, value);
        }
    }
}