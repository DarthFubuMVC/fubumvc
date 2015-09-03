using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Shouldly
{
    public static class Exception<T> where T : Exception
    {
        public static T ShouldBeThrownBy(Action action)
        {
            T exception = null;

            try
            {
                action();
            }
            catch (Exception e)
            {
                exception = e.ShouldBeOfType<T>();
            }

            if (exception == null)
                throw new Exception("An exception was expected, but not thrown by the given action.");

            return exception;
        }
    }


    public delegate void MethodThatThrows();

    public static class SpecificationExtensions
    {
        public static void ShouldBeTrue(this bool anObject)
        {
            anObject.ShouldBe(true);
        }

        public static void ShouldBeFalse(this bool anObject)
        {
            anObject.ShouldBe(false);
        }

        public static void ShouldBeNull(this object anObject)
        {
            anObject.ShouldBe(null);
        }

        public static T ShouldNotBeNull<T>(this T anObject) where T : class
        {
            anObject.ShouldNotBe(null);
            return anObject;
        }


        public static object ShouldBeTheSameAs(this object actual, object expected)
        {
            ReferenceEquals(actual, expected).ShouldBe(true);
            return expected;
        }

        public static object ShouldNotBeTheSameAs(this object actual, object expected)
        {
            ReferenceEquals(actual, expected).ShouldBe(false);
            return expected;
        }

        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, params T[] expected)
        {
            actual.ShouldBe(expected);
        }

        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> enumerable, int expected)
        {
            enumerable.Count().ShouldBe(expected);
            return enumerable;
        }

        public static void ShouldHaveTheSameElementsAs(this IList actual, IList expected)
        {
            try
            {
                actual.ShouldNotBeNull();
                expected.ShouldNotBeNull();

                actual.Count.ShouldBe(expected.Count);

                for (var i = 0; i < actual.Count; i++)
                {
                    actual[i].ShouldBe(expected[i]);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Actual values were:");
                actual.Each(x => Debug.WriteLine(x));
                throw;
            }
        }


        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            var actualList = (actual is IList) ? (IList)actual : actual.ToList();
            var expectedList = (expected is IList) ? (IList)expected : expected.ToList();

            ShouldHaveTheSameElementsAs(actualList, expectedList);
        }
    }


/*

    public static class Exception<T> where T : Exception
    {
        public static T ShouldBeThrownBy(Action action)
        {
            T exception = null;

            try
            {
                action();
            }
            catch (Exception e)
            {
                exception = e.ShouldBeOfType<T>();
            }

            if (exception == null) Assert.Fail("An exception was expected, but not thrown by the given action.");

            return exception;
        }
    }


    public delegate void MethodThatThrows();

    public static class SpecificationExtensions
    {
        public static void ShouldHave<T>(this IEnumerable<T> values, Func<T, bool> func)
        {
            values.FirstOrDefault(func).ShouldNotBeNull();
        }

        public static void ShouldNotHave<T>(this IEnumerable<T> values, Func<T, bool> func)
        {
            values.FirstOrDefault(func).ShouldBeNull();
        }

        public static void ShouldBeFalse(this bool condition)
        {
            Assert.IsFalse(condition);
        }

        public static void ShouldBeTrue(this bool condition)
        {
            Assert.IsTrue(condition);
        }

        public static object ShouldBe(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
            return expected;
        }

        public static object ShouldBe(this string actual, object expected)
        {
            Assert.AreEqual((expected != null) ? expected.ToString() : null, actual);
            return expected;
        }

        public static object ShouldNotBe(this object actual, object expected)
        {
            Assert.AreNotEqual(expected, actual);
            return expected;
        }

        public static void ShouldBeNull(this object anObject)
        {
            Assert.IsNull(anObject);
        }

        public static T ShouldNotBeNull<T>(this T anObject)
        {
            Assert.IsNotNull(anObject);
            return anObject;
        }

        public static void ShouldNotBeNull(this object anObject, string message)
        {
            Assert.IsNotNull(anObject, message);
        }

        public static object ShouldBeTheSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
            return expected;
        }

        public static object ShouldNotBeTheSameAs(this object actual, object expected)
        {
            Assert.AreNotSame(expected, actual);
            return expected;
        }

        public static T ShouldBeOfType<T>(this object actual)
        {
            actual.ShouldNotBeNull();
            actual.ShouldBeOfType(typeof (T));
            return (T) actual;
        }


        public static object ShouldBeOfType(this object actual, Type expected)
        {
            Assert.IsInstanceOf(expected, actual);
            return actual;
        }

        public static void ShouldNotBeOfType(this object actual, Type expected)
        {
            Assert.IsNotInstanceOf(expected, actual);
        }

        public static void ShouldNotBeOfType<T>(this object actual)
        {
            Assert.IsNotInstanceOf(typeof (T), actual);
        }

        public static void ShouldContain(this IList actual, object expected)
        {
            Assert.Contains(expected, actual);
        }

        public static void ShouldContain<T>(this IEnumerable<T> actual, T expected)
        {
            if (actual.Count(t => t.Equals(expected)) == 0)
            {
                Assert.Fail("The item '{0}' was not found in the sequence.", expected);
            }
        }

        public static void ShouldNotBeEmpty<T>(this IEnumerable<T> actual)
        {
            Assert.Greater(actual.Count(), 0, "The list should have at least one element");
        }

        public static void ShouldNotContain<T>(this IEnumerable<T> actual, T expected)
        {
            if (actual.Count(t => t.Equals(expected)) > 0)
            {
                Assert.Fail("The item was found in the sequence it should not be in.");
            }
        }

        public static void ShouldHaveTheSameElementsAs(this IList actual, IList expected)
        {
            try
            {
                actual.ShouldNotBeNull();
                expected.ShouldNotBeNull();

                actual.Count.ShouldBe(expected.Count);

                for (var i = 0; i < actual.Count; i++)
                {
                    actual[i].ShouldBe(expected[i]);
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("Actual values were:");
                actual.Each(x => Debug.WriteLine(x));
                throw;
            }
        }

        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, params T[] expected)
        {
            ShouldHaveTheSameElementsAs(actual, (IEnumerable<T>) expected);
        }

        public static void ShouldHaveTheSameElementsAs<T>(this IEnumerable<T> actual, IEnumerable<T> expected)
        {
            var actualList = (actual is IList) ? (IList) actual : actual.ToList();
            var expectedList = (expected is IList) ? (IList) expected : expected.ToList();

            ShouldHaveTheSameElementsAs(actualList, expectedList);
        }

        public static void ShouldHaveTheSameElementKeysAs<ELEMENT, KEY>(this IEnumerable<ELEMENT> actual,
            IEnumerable expected,
            Func<ELEMENT, KEY> keySelector)
        {
            actual.ShouldNotBeNull();
            expected.ShouldNotBeNull();

            var actualArray = actual.ToArray();
            var expectedArray = expected.Cast<object>().ToArray();

            actualArray.Length.ShouldBe(expectedArray.Length);

            for (var i = 0; i < actual.Count(); i++)
            {
                keySelector(actualArray[i]).ShouldBe(expectedArray[i]);
            }
        }

        public static IComparable ShouldBeGreaterThan(this IComparable arg1, IComparable arg2)
        {
            Assert.Greater(arg1, arg2);
            return arg2;
        }

        public static IComparable ShouldBeLessThan(this IComparable arg1, IComparable arg2)
        {
            Assert.Less(arg1, arg2);
            return arg2;
        }

        public static void ShouldBeEmpty(this ICollection collection)
        {
            Assert.IsEmpty(collection);
        }

        public static void ShouldBeEmpty(this string aString)
        {
            Assert.IsEmpty(aString);
        }

        public static void ShouldNotBeEmpty(this string aString)
        {
            Assert.IsNotEmpty(aString);
        }

        public static void ShouldContain(this string actual, string expected)
        {
            StringAssert.Contains(expected, actual);
        }


        public static void ShouldEndWith(this string actual, string expected)
        {
            StringAssert.EndsWith(expected, actual);
        }

        public static void ShouldStartWith(this string actual, string expected)
        {
            StringAssert.StartsWith(expected, actual);
        }


        public static Exception ShouldBeThrownBy(this Type exceptionType, MethodThatThrows method)
        {
            Exception exception = null;

            try
            {
                method();
            }
            catch (Exception e)
            {
                Assert.AreEqual(exceptionType, e.GetType());
                exception = e;
            }

            if (exception == null)
            {
                Assert.Fail("Expected {0} to be thrown.", exceptionType.FullName);
            }

            return exception;
        }


        public static IEnumerable<T> ShouldHaveCount<T>(this IEnumerable<T> actual, int expected)
        {
            actual.Count().ShouldBe(expected);
            return actual;
        }


        public class CapturingConstraint : AbstractConstraint
        {
            private readonly ArrayList argList = new ArrayList();

            public override string Message
            {
                get { return ""; }
            }

            public override bool Eval(object obj)
            {
                argList.Add(obj);
                return true;
            }

            public T First<T>()
            {
                return ArgumentAt<T>(0);
            }

            public T ArgumentAt<T>(int pos)
            {
                return (T) argList[pos];
            }

            public T Second<T>()
            {
                return ArgumentAt<T>(1);
            }
        }
    }
 * */
}