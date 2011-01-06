using System;
using System.Collections;
using System.Collections.Generic;
using FubuCore.Util;
using NUnit.Framework;
using FubuCore;
using Rhino.Mocks;

namespace FubuCore.Testing.Util
{
    [TestFixture]
    public class CacheTester
    {
        private Cache<string, int> cache;
        private const string Key = "someKey";
        public interface ICallback
        {
            string GetKeyCallback(int value);
            void OnAdditionCallback(int value);
        }

        [SetUp]
        public void SetUp()
        {
            cache = new Cache<string, int>();
        }

        [Test]
        public void on_addition_should_fire_when_a_cache_adds_something_from_its_on_missing_catch()
        {
            var list = new List<int>();
            int x = 0;

            cache.OnMissing = key => ++x;

            cache.OnAddition = number => list.Add(number);

            cache["a"] = 100;
            cache["b"].ShouldEqual(1);
            cache["c"].ShouldEqual(2);
        
            list.ShouldHaveTheSameElementsAs(100, 1, 2);
        }

        [Test]
        public void when_GetKey_not_set_should_throw()
        {
            typeof (NotImplementedException).ShouldBeThrownBy(() => cache.GetKey(2));
        }

        [Test]
        public void when_key_not_found_should_throw_by_default()
        {
            int i;
            const string key = "nonexisting key";
            typeof (KeyNotFoundException).ShouldBeThrownBy(() => i = cache[key]).
                Message.ShouldEqual("Key '{0}' could not be found".ToFormat(key));
        }

        [Test]
        public void predicate_exists()
        {
            cache.Fill(Key, 42);
            cache.Exists(i => i == 42).ShouldBeTrue();
        }

        [Test]
        public void predicate_finds()
        {
            cache.Fill(Key, 42);
            cache.Find(i => i == 42).ShouldEqual(42);
            cache.Find(i => i == 43).ShouldEqual(0);
        }

        [Test]
        public void get_first_value()
        {
            cache.Fill(Key, 42);
            cache.Fill("anotherKey", 99);
            cache.First.ShouldEqual(42);
            cache.ClearAll();
            cache.First.ShouldEqual(0);
        }

        [Test]
        public void get_all_keys()
        {
            cache.Fill(Key, 42);
            cache.GetAllKeys().ShouldHaveCount(1).ShouldContain(Key);
        }

        [Test]
        public void get_enumerator()
        {
            cache.Fill(Key, 42);
            cache.GetEnumerator().ShouldBeOfType(typeof(IEnumerator<int>));
            IEnumerable enumerable = cache;
            enumerable.GetEnumerator().ShouldBeOfType(typeof (IEnumerator));
            cache.ShouldHaveCount(1).ShouldContain(42);
        }

        [Test]
        public void set_GetKey()
        {
            ICallback callback = MockRepository.GenerateStub<ICallback>();
            cache.GetKey = callback.GetKeyCallback;
            cache.GetKey(42);
            callback.AssertWasCalled(c=>c.GetKeyCallback(42));
        }

        [Test]
        public void set_OnAddition()
        {
            ICallback callback = MockRepository.GenerateStub<ICallback>();
            cache["firstKey"] = 0;
            callback.AssertWasNotCalled(c => c.OnAdditionCallback(42));
            cache.OnAddition = callback.OnAdditionCallback;
            cache[Key] = 42;
            callback.AssertWasCalled(c=>c.OnAdditionCallback(42));
        }

        [Test]
        public void can_remove()
        {
            cache[Key] = 42;
            cache.Has(Key).ShouldBeTrue();
            cache.Remove(Key);
            cache.Has(Key).ShouldBeFalse();
        }

        [Test]
        public void store_and_fetch()
        {
            cache["a"] = 1;
            cache["a"].ShouldEqual(1);

            cache["a"] = 2;
            cache["a"].ShouldEqual(2);
        }

        [Test]
        public void test_the_on_missing()
        {
            int count = 0;
            cache.OnMissing = key => ++count;


            cache["a"].ShouldEqual(1);
            cache["b"].ShouldEqual(2);
            cache["c"].ShouldEqual(3);

            cache["a"].ShouldEqual(1);
            cache["b"].ShouldEqual(2);
            cache["c"].ShouldEqual(3);

            cache.Count.ShouldEqual(3);
        }

        [Test]
        public void fill_only_writes_if_there_is_not_previous_value()
        {
            cache.Fill("a", 1);
            cache["a"].ShouldEqual(1);

            cache.Fill("a", 2);
            cache["a"].ShouldEqual(1); // did not overwrite
        }

        [Test]
        public void WithValue_positive()
        {
            cache["b"] = 2;

            int number = 0;

            cache.WithValue("b", i => number = i);

            number.ShouldEqual(2);
        }

        [Test]
        public void WithValue_negative()
        {
            cache.WithValue("b", i => Assert.Fail("Should not be called"));
        }
    }
}