using FubuCore.Binding;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.Binding
{
    [TestFixture]
    public class InMemoryRequestDataTester
    {
        public interface ICalledUpon{void Action();}
        private InMemoryRequestData _data;
        private ICalledUpon _calledUpon;

        [SetUp]
        public void SetUp()
        {
            _calledUpon = MockRepository.GenerateStub<ICalledUpon>();
            _data = new InMemoryRequestData();
            _calledUpon.Stub(c => c.Action());
        }

        [Test]
        public void should_cache_value()
        {
            const int value = 6;
            _data["key"] = value;
            _data["key"].ShouldEqual(value);
            _data.Value("key").ShouldEqual(value);
            _data.Value("key", o => _calledUpon.Action());
            _calledUpon.AssertWasCalled(c=>c.Action());
            _data.Value("non_existing_key").ShouldBeNull();
        }
    }
}