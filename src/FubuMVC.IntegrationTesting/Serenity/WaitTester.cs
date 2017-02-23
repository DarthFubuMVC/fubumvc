using System;
using Xunit;
using OpenQA.Selenium;
using Serenity;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Serenity
{
    
    public class WaitTester
    {
        [Fact]
        public void immediately_true()
        {
            Wait.Until(() => true).
                ShouldBeTrue();
        }

        [Fact]
        public void always_going_to_be_false()
        {
            Wait.Until(() => false, timeoutInMilliseconds:20, millisecondPolling:5)
                .ShouldBeFalse();
        }

        [Fact]
        public void eventually_true()
        {
            int i = 0;

            Wait.Until(() => {
                i++;
                return i > 4;
            }, millisecondPolling:5)
            .ShouldBeTrue();
        }

        [Fact]
        public void not_true_within_timeout()
        {
            int i = 0;

            Wait.Until(() => {
                i++;
                return i > 4;
            }, timeoutInMilliseconds: 50, millisecondPolling:20)
            .ShouldBeFalse();

            i.ShouldBeGreaterThan(1);
        }

        [Fact]
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

        [Fact]
        public void false_prevents_follow_on_conditions()
        {
            Wait.Until(new Func<bool>[]
            {
                () => true,
                () => false,
                () => { throw new Exception("Should never reach this exception"); }
            }, timeoutInMilliseconds: 1000).ShouldBeFalse();
        }

        [Fact]
        public void eventually_true_many_conditions()
        {
            var i = 0;

            Wait.Until(new Func<bool>[]
            {
                () => { i++; return i > 2; },
                () => { i++; return i > 4; },
                () => { i++; return i > 6; },
                () => { i++; return i > 8; },
            }, millisecondPolling:5).ShouldBeTrue();
        }

        [Fact]
        public void not_true_within_timeout_many_conditions()
        {
            var i = 0;

            Wait.Until(new Func<bool>[]
            {
                () => { i++; return i > 4; },
                () => { i++; return i > 8; },
                () => { i++; return i > 12; },
                () => { i++; return i > 16; },
            }, millisecondPolling:5, timeoutInMilliseconds:50).ShouldBeFalse();

            i.ShouldBeGreaterThan(4);
        }

        [Fact]
        public void immediately_true_generic()
        {
            var obj = new object();
            Wait.For(() => obj).ShouldBeTheSameAs(obj);
        }

        [Fact]
        public void throws_timeout_exception_generic()
        {
            var result = new object();
            Exception<WebDriverTimeoutException>.ShouldBeThrownBy(() =>
            {
                result = Wait.For<object>(() => null, timeout: TimeSpan.FromMilliseconds(100));
            });

            result.ShouldNotBeNull();
        }

        [Fact]
        public void eventually_true_generic()
        {
            var i = 0;
            var obj = new object();

            Wait.For(() => {
                i++;
                return i > 4 ? obj : null;
            }, pollingInterval:TimeSpan.FromMilliseconds(5)).ShouldBeTheSameAs(obj);

            i.ShouldBeGreaterThan(4);
        }

        [Fact]
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