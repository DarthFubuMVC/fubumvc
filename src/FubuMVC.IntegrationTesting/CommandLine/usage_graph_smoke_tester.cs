using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.CommandLine;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.IntegrationTesting.CommandLine
{
    [TestFixture]
    public class usage_graph_smoke_tester
    {
        [Test]
        public void all_commands_usage_graph_works()
        {
            var factory = new CommandFactory();
            factory.RegisterCommands(typeof(IFubuCommand).Assembly);

            factory.AllCommandTypes().Each(t =>
            {

                try
                {
                    var usageGraph = new UsageGraph(t);
                    usageGraph.WriteUsages("fubu");

                    usageGraph.Usages.Any().ShouldBeTrue();

                }
                catch (Exception e)
                {
                    throw new ApplicationException("Command type:  " + t.FullName, e);
                }
            });
        }

        [Test]
        public void vdir()
        {
            new UsageGraph(typeof(CreateVdirCommand)).WriteUsages("fubu");
        }

        public class FakeInput{}
        class CreateVdirCommand : FubuCommand<FakeInput>
        {
            public override bool Execute(FakeInput input)
            {
                return true;
            }
        }
    }
}