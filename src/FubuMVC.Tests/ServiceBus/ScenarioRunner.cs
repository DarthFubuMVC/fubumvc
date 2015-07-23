using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Tests.ServiceBus.ScenarioSupport;
using FubuTransportation.Testing.Scenarios;
using NUnit.Framework;
using System.Linq;
using FubuCore;
using Shouldly;

namespace FubuTransportation.Testing
{
    [TestFixture]
    public class ScenarioRunner
    {
        [Test, Explicit]
        public void write_previews()
        {
            var scenarios = FindScenarios();
            var writer = new ScenarioWriter();

            scenarios.Each(x => {
                x.Preview(writer);
                writer.BlankLine();
                writer.BlankLine();
            });
        }

        [Test]
        public void send_a_single_message_to_the_correct_node()
        {
            var writer = new ScenarioWriter();

            new Send_a_single_message_to_the_correct_node().Execute(writer);

            writer.FailureCount.ShouldBe(0);
        }

        [Test]
        public void Send_a_single_message_to_multiple_listening_nodes()
        {
            var writer = new ScenarioWriter();

            new Send_a_single_message_to_multiple_listening_nodes().Execute(writer);

            writer.FailureCount.ShouldBe(0);
        }

        [Test]
        public void Send_a_message_that_raises_events()
        {
            var writer = new ScenarioWriter();

            new Send_a_message_that_raises_events().Execute(writer);

            writer.FailureCount.ShouldBe(0);
        }

        [Test]
        public void Request_a_reply_for_a_single_message()
        {
            var writer = new ScenarioWriter();

            new Request_a_reply_for_a_single_message().Execute(writer);

            writer.FailureCount.ShouldBe(0);
        }

        [Test]
        public void send_and_await_for_a_single_message()
        {
            var writer = new ScenarioWriter();

            new SendAndAwait_for_a_single_message().Execute(writer);

            writer.FailureCount.ShouldBe(0);
        }


        [Test, Explicit]
        public void run_all_scenarios()
        {
            var scenarios = FindScenarios();
            var failures = new List<string>();
            
            scenarios.Each(x => {
                var writer = new ScenarioWriter();

                x.Execute(writer);

                if (writer.FailureCount > 0)
                {
                    failures.Add(x.Title);

                    Console.WriteLine(writer.ToString());
                }
            });

            if (failures.Any())
            {
                Debug.WriteLine("Scenarios failed!");
                failures.Each(x => Debug.WriteLine(x));

                Assert.Fail();
            }
        }

        public static IEnumerable<Scenario> FindScenarios()
        {
            return Assembly.GetExecutingAssembly()
                           .GetTypes()
                           .Where(x => x.IsConcreteTypeOf<Scenario>() && x != typeof (Scenario))
                           .Select(x => {
                               return typeof (Builder<>).CloseAndBuildAs<IScenarioBuilder>(x).Build();
                           });

        }

        public interface IScenarioBuilder
        {
            Scenario Build();
        }

        public class Builder<T> : IScenarioBuilder where T : Scenario, new()
        {
            public Scenario Build()
            {
                return new T();
            }
        }
    }
}