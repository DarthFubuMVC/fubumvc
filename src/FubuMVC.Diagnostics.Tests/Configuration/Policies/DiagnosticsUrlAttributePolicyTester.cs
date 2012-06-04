using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Diagnostics.Core.Configuration.Policies;
using FubuMVC.Diagnostics.Runtime;
using FubuMVC.Tests;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Diagnostics.Tests.Configuration.Policies
{
    public class DiagnosticsUrlAttributePolicyTester
    {
        private DiagnosticsAttributeUrlPolicy _policy;
        private IConfigurationObserver _observer;

        [SetUp]
        public void Setup()
        {
            _policy = new DiagnosticsAttributeUrlPolicy();
            _observer = new NulloConfigurationObserver();
        }

        [Test]
        public void should_match_calls_with_diagnostics_url_attribute()
        {
            var callWithAttribute = ActionCall.For<DiagnosticsCallWithAttribute>(c => c.Execute());
            _policy
                .Matches(callWithAttribute, _observer)
                .ShouldBeTrue();
        }

        [Test]
        public void should_not_match_calls_without_diagnostics_url_attribute()
        {
            var invalidCall = ActionCall.For<InvalidClass>(c => c.Execute());
            _policy
                .Matches(invalidCall, _observer)
                .ShouldBeFalse();
        }

        [Test]
        public void should_make_url_relative()
        {
            var call = ActionCall.For<DiagnosticsCallWithAttribute>(c => c.Execute());
            _policy
                .Build(call)
                .Pattern
                .ShouldEqual("{0}/my-extension".ToFormat(DiagnosticsUrls.ROOT));
        }


        public class InvalidClass
        {
            public void Execute()
            {
            }
        }   
    }
}