using System;
using Shouldly;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Serenity.Testing
{
    [TestFixture]
    public class WaitTester
    {
        [Test]
        public void immediately_true()
        {
            Wait.Until(() => true).
                ShouldBeTrue();
        }

        [Test]
        public void always_going_to_be_false()
        {
            Wait.Until(() => false, timeoutInMilliseconds:1000)
                .ShouldBeFalse();
        }

        [Test]
        public void eventually_true()
        {
            int i = 0;

            Wait.Until(() => {
                i++;
                return i > 4;
            })
            .ShouldBeTrue();
        }

        [Test]
        public void not_true_within_timeout()
        {
            int i = 0;

            Wait.Until(() => {
                i++;
                return i > 4;
            }, timeoutInMilliseconds: 1000)
            .ShouldBeFalse();

            i.ShouldBeGreaterThan(1);
        }

        [Test]
        public void immediately_true_many_conditions()
        {
            Wait.Until(new Func<bool>[]
            {
                () => true,
                () => true,
                () => true,
                () => true
            }).ShouldBeTrue();
        }

        [Test]
        public void false_prevents_follow_on_conditions()
        {
            Wait.Until(new Func<bool>[]
            {
                () => true,
                () => false,
                () => { throw new Exception("Should never reach this exception"); }
            }, timeoutInMilliseconds: 1000).ShouldBeFalse();
        }

        [Test]
        public void eventually_true_many_conditions()
        {
            var i = 0;

            Wait.Until(new Func<bool>[]
            {
                () => { i++; return i > 2; },
                () => { i++; return i > 4; },
                () => { i++; return i > 6; },
                () => { i++; return i > 8; },
            }).ShouldBeTrue();
        }

        [Test]
        public void not_true_within_timeout_many_conditions()
        {
            var i = 0;

            Wait.Until(new Func<bool>[]
            {
                () => { i++; return i > 4; },
                () => { i++; return i > 8; },
                () => { i++; return i > 12; },
                () => { i++; return i > 16; },
            }).ShouldBeFalse();

            i.ShouldBeGreaterThan(4);
        }

        [Test]
        public void immediately_true_generic()
        {
            var obj = new object();
            Wait.For(() => obj).ShouldBeTheSameAs(obj);
        }

        [Test]
        public void throws_timeout_exception_generic()
        {
            var result = new object();
            Exception<WebDriverTimeoutException>.ShouldBeThrownBy(() =>
            {
                result = Wait.For<object>(() => null, timeout: TimeSpan.FromSeconds(1));
            });

            result.ShouldNotBeNull();
        }

        [Test]
        public void eventually_true_generic()
        {
            var i = 0;
            var obj = new object();

            Wait.For(() => {
                i++;
                return i > 4 ? obj : null;
            }).ShouldBeTheSameAs(obj);

            i.ShouldBeGreaterThan(4);
        }

        [Test]
        public void not_true_within_timeout_generic()
        {
            var i = 0;
            var result = new object();

            Exception<WebDriverTimeoutException>.ShouldBeThrownBy(() =>
            {
                result = Wait.For(() =>
                {
                    i++;
                    return i > 4 ? new object() : null;
                }, timeout: TimeSpan.FromSeconds(1));
            });

            i.ShouldBeGreaterThan(1);
            result.ShouldNotBeNull();
        }
    }
}