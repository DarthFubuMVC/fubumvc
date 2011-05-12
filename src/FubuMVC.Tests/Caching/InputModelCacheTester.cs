using System;
using FubuCore;
using FubuMVC.Core.Caching;
using FubuMVC.Core.Runtime;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class InputModelCacheTester : InteractionContext<RequestOutputCache<TestObj>>
    {
        private bool generatorWasCalled;
        private Func<TestObj, RecordedOutput> recordAction;
        private TestObj theModel;
        string theKey = "Brandon";
        bool wasCalled = false;
        CacheOptions<TestObj> cacheOptions = new CacheOptions<TestObj>(obj => obj.Name);

        protected override void beforeEach()
        {
            theModel = new TestObj() { Name = theKey };
            
            generatorWasCalled = false;
            
            Services.Inject(cacheOptions);

            recordAction = obj =>
            {
                generatorWasCalled = true;
                return new RecordedOutput("test/html", "View:{0}".ToFormat(obj.Name));
            };

        }

        private void execute()
        {
            ClassUnderTest.WithCache(theModel,
                                     recordAction,
                                     cache=>
                                     {
                                         wasCalled = true;
                                     });
        }

        [Test]
        public void should_generate_data_on_cache_miss()
        {
            
            var theValue = new RecordedOutput("test/html", "View:Brandon");
            

            MockFor<ICacheProvider>().Stub(cp => cp.Get(theKey)).Return(null);

            execute();

            MockFor<ICacheProvider>().AssertWasCalled(cp=>cp.Insert(theKey, theValue, cacheOptions.Dependency, cacheOptions.AbsoluteExpiration, cacheOptions.SlidingExpiration));

            generatorWasCalled.ShouldBeTrue();
            wasCalled.ShouldBeTrue();
        }


        [Test]
        public void should_not_generate_data_on_cache_hit()
        {
            var theValue = new RecordedOutput("test/html", "View:Brandon");

            MockFor<ICacheProvider>().Stub(cp => cp.Get(theKey)).Return(theValue);

            execute();

            MockFor<ICacheProvider>().AssertWasNotCalled(cp => cp.Insert(theKey, theValue, cacheOptions.Dependency, cacheOptions.AbsoluteExpiration, cacheOptions.SlidingExpiration));

            generatorWasCalled.ShouldBeFalse();
            wasCalled.ShouldBeTrue();
        }
    }
        public class TestObj
        {
            public string Name { get; set; }

            public bool Equals(TestObj other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Name, Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (TestObj)) return false;
                return Equals((TestObj) obj);
            }

            public override int GetHashCode()
            {
                return (Name != null ? Name.GetHashCode() : 0);
            }
        }
}