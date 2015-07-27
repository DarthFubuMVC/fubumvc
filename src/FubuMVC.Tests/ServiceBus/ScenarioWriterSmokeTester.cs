using System;
using System.Diagnostics;
using FubuMVC.Core.ServiceBus;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus
{
    [TestFixture]
    public class ScenarioWriterSmokeTester
    {
        private ScenarioWriter writer;

        [SetUp]
        public void SetUp()
        {
            writer = new ScenarioWriter();
        }

        [TearDown]
        public void TearDown()
        {
            Debug.WriteLine(writer.ToString());
        }

        [Test]
        public void simple_write_line()
        {
            writer.WriteLine("I'm a single line");
        }

        [Test]
        public void write_title()
        {
            writer.WriteTitle("I'm the title");
        }

        [Test]
        public void write_a_blank_line()
        {
            writer.WriteLine("AAAAAAAAAAAAA");
            writer.BlankLine();
            writer.WriteLine("BBBBBBBBBBBBB");
        }

        [Test]
        public void write_with_indention()
        {
            writer.WriteLine("AAAAAAAAAAAAA");
            using (writer.Indent())
            {
                writer.WriteLine("BBBBBBBBBBBBB");
            }
            writer.WriteLine("CCCCCCCCCCCCC");
        }

        [Test]
        public void write_exception()
        {
            var ex = new NotImplementedException();

            writer.Exception(ex);

            writer.FailureCount.ShouldBe(1);
        }

        [Test]
        public void failure()
        {
            writer.Failure("You stink!");

            writer.FailureCount.ShouldBe(1);
        }
    }
}