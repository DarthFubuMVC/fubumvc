using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Bugs
{
    [TestFixture]
    public class DoubleActionRegistrationWhenDiagnosticsIsOnTester
    {
        [TestFixture]
        public class ActionSourcesTests
        {
            [Test]
            public void oh_noes()
            {
                build(false).Each(a => Console.WriteLine(a)).ShouldHaveCount(2);
            }

            [Test]
            public void yay()
            {
                build(true).Each(a => Console.WriteLine(a)).ShouldHaveCount(2);
            }


            private static IEnumerable<ActionCall> build(bool enableDiagnostics)
            {
                var graph = BehaviorGraph.BuildFrom(new CustomFubuRegistry(enableDiagnostics));

                return graph.Actions()
                    .Where(a => a.HandlerType.Assembly == typeof(CustomFubuRegistry).Assembly);
            }
        }
    }

    public class CustomFubuRegistry : FubuRegistry
    {
        public CustomFubuRegistry() : this(false) { }
        public CustomFubuRegistry(bool diagnostics)
        {
            Applies.ToThisAssembly();

            Actions
                .IncludeTypesNamed(t => t.EndsWith("Python"))
                .IncludeMethods(m => m.Name.Equals("Command"))
                .IncludeMethods(m => m.Name.Equals("Query"));
        }
    }


    public class ChuckNorris
    {
        public RoundKick Query()
        {
            return new RoundKick();
        }

        public class RoundKick { }
    }

    public class MontyPython
    {
        public OutQ Query(InQ inQ)
        {
            return new OutQ();
        }

        public void Command(InC inC)
        {

        }

        public class InQ { }
        public class OutQ { }
        public class InC { }

    }

}