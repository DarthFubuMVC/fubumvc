using System;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Services;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.Diagnostics
{
    [TestFixture]
    public class end_to_end_execution_log_tracing_for_http_requests
    {
        [Test]
        public void see_the_trace_messages_going_through()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.Features.Diagnostics.Enable(TraceLevel.Production)))
            {
                var id1 = runtime.Scenario(_ =>
                {
                    _.Get.Action<DiagnosticEndpoints>(x => x.get_message1());
                }).RequestId();

                var id2 = runtime.Scenario(_ =>
                {
                    _.Get.Action<DiagnosticEndpoints>(x => x.get_message1());
                }).RequestId();

                var id3 = runtime.Scenario(_ =>
                {
                    _.Get.Action<DiagnosticEndpoints>(x => x.get_message2());
                }).RequestId();

                var id4 = runtime.Scenario(_ =>
                {
                    _.Get.Action<DiagnosticEndpoints>(x => x.get_message2());
                }).RequestId();

                var id5 = runtime.Scenario(_ =>
                {
                    _.Get.Action<DiagnosticEndpoints>(x => x.get_message2());
                }).RequestId();

                var history = runtime.Get<IChainExecutionHistory>();

                var success = Wait.Until(() => history.RecentReports().Count() == 5);
                success.ShouldBeTrue();

                history.Find(Guid.Parse(id1)).ShouldNotBeNull();
                history.Find(Guid.Parse(id2)).ShouldNotBeNull();
                history.Find(Guid.Parse(id3)).ShouldNotBeNull();
                history.Find(Guid.Parse(id4)).ShouldNotBeNull();
                history.Find(Guid.Parse(id5)).ShouldNotBeNull();

                Wait.Until(() =>
                {
                    return runtime.Behaviors.ChainFor<DiagnosticEndpoints>(x => x.get_message1())
                        .Performance.HitCount == 2;
                }).ShouldBeTrue();

                Wait.Until(() =>
                {
                    return runtime.Behaviors.ChainFor<DiagnosticEndpoints>(x => x.get_message2())
                        .Performance.HitCount == 3;
                }).ShouldBeTrue();


            }
        }

        [Test]
        public void log_partial_execution()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.Features.Diagnostics.Enable(TraceLevel.Verbose)))
            {
                var id = runtime.Scenario(_ =>
                {
                    _.Get.Url("with/partial");
                }).RequestId();

                var history = runtime.Get<IChainExecutionHistory>();

                Wait.Until(() => history.RecentReports().Any()).ShouldBeTrue();

                var log = history.Find(Guid.Parse(id));

                log.ShouldNotBeNull();

                var chain = runtime.Behaviors.ChainFor<DiagnosticEndpoints>(x => x.get_response1(null));
                log.Activity.Nested.Single().Nested.Single(x => x.Subject is BehaviorChain).Subject.ShouldBe(chain);
            }
        }
    }

    public class DiagnosticEndpoints
    {
        private readonly IPartialInvoker _invoker;

        public DiagnosticEndpoints(IPartialInvoker invoker)
        {
            _invoker = invoker;
        }

        public string get_message1()
        {
            return "Message1";
        }

        public string get_response1(Request1 request)
        {
            return request.Name;
        }

        public string get_with_partial()
        {
            return _invoker.InvokeAsHtml(new Request1 {Name = "Tamba Hali"});
        }

        public class Request1
        {
            public string Name { get; set; }
        }

        public class Response1
        {
            public string Name { get; set; }
        }

        public string get_message2()
        {
            return "Message2";
        }
    }
}