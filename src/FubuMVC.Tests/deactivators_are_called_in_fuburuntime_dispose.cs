using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using Shouldly;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class deactivators_are_called_in_fuburuntime_dispose
    {
        [Test]
        public void disposing_the_fubu_runtime_should_run_all_the_deactivators()
        {
            FakeDeactivator.Messages.Clear();

            var container = new Container(x =>
            {
                x.For<IDeactivator>().Add(new FakeDeactivator("red"));
                x.For<IDeactivator>().Add(new FakeDeactivator("green"));
                x.For<IDeactivator>().Add(new FakeDeactivator("blue"));
            });

            FubuRuntime.Basic(x => x.StructureMap(container)).Dispose();

            FakeDeactivator.Messages.ShouldHaveTheSameElementsAs("red", "green", "blue");
        }
    }

    public class FakeDeactivator : IDeactivator
    {
        private readonly string _name;
        public static readonly IList<string> Messages = new List<string>();

        public FakeDeactivator(string name)
        {
            _name = name;
        }

        public void Deactivate(IActivationLog log)
        {
            Messages.Add(_name);
        }
    }
}