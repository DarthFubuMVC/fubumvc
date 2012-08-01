using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http.AspNet;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Http.AspNet
{
    [TestFixture]
    public class SimpleKeyValueTester
    {
         
        private IDictionary<string, object> _ajaxRequestInput = new Dictionary<string, object> { { "X-Requested-With", "XMLHttpRequest" } };
        
        [Test]
        public void should_default_to_simple_contains()
        {
            var skv = new SimpleKeyValues((key) => _ajaxRequestInput[key], () => _ajaxRequestInput.Select(kvp => kvp.Key));

            skv.Has("X-Requested-With").ShouldBeTrue();
            skv.Has("x-requested-with").ShouldBeFalse();

        }
        [Test]
        public void should_use_custom_compare()
        {
            var skv = new SimpleKeyValues((key) => _ajaxRequestInput[key], () => _ajaxRequestInput.Select(kvp => kvp.Key), (key,keys) => keys.Contains(key,StringComparer.InvariantCultureIgnoreCase));

            skv.Has("X-Requested-With").ShouldBeTrue();
            skv.Has("x-requested-with").ShouldBeTrue();

        }
    }
}