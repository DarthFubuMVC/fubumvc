using System.Collections.Generic;
using FubuCore;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Http.Owin
{
    [TestFixture]
    public class DictionaryExtensionsTester
    {
        [Test]
        public void copy_multiple_keys_that_all_exist_in_source()
        {
            var source = new Dictionary<string, object>();
            source.Add("a", 1);
            source.Add("b", 2);
            source.Add("c", 3);

            var destination = new Dictionary<string, object>();

            source.CopyTo(destination, "a", "b", "c");

            destination["a"].ShouldBe(1);
            destination["b"].ShouldBe(2);
            destination["c"].ShouldBe(3);
        }

        [Test]
        public void copy_multiple_keys_with_some_misses()
        {
            var source = new Dictionary<string, object>();
            source.Add("a", 1);
            //source.Add("b", 1);
            source.Add("c", 3);

            var destination = new Dictionary<string, object>();

            source.CopyTo(destination, "a", "b", "c");

            destination["a"].ShouldBe(1);
            destination.ContainsKey("b").ShouldBeFalse();
            destination["c"].ShouldBe(3);
        }

        [Test]
        public void set_and_read_request_id()
        {
            var dict = new Dictionary<string, object>();
            dict.RequestId().ShouldBeNull();

            dict.RequestId("abc");

            dict.RequestId().ShouldBe("abc");
        }


        [Test]
        public void set_the_chain_execution_log()
        {
            var source = new Dictionary<string, object>();
            source.Log().ShouldBeNull();

            var log = new ChainExecutionLog();

            source.Log(log);

            source.RequestId().ShouldBe(log.Id.ToString());

            source.Log().ShouldBeSameAs(log);
        }
    }
}