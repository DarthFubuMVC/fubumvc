using System;
using System.IO;
using FubuMVC.Core.View.Rendering;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class NestedOutputTester : InteractionContext<NestedOutput>
    {
        private TextWriter _writer;
		protected override void beforeEach()
		{
		    _writer = new StringWriter();
		}

        [Test]
        public void if_the_writer_has_not_been_set_is_active_returns_false()
        {
            ClassUnderTest.IsActive().ShouldBeFalse();
        }

        [Test]
        public void if_the_writer_has_been_set_is_active_returns_true()
        {
            ClassUnderTest.SetWriter(() => _writer);
            ClassUnderTest.IsActive().ShouldBeTrue();
        }

        [Test]
        public void writer_executes_the_delegate_passed_on_set_writer()
        {
            Func<TextWriter> writerFunc = () => _writer;
            ClassUnderTest.SetWriter(writerFunc);
            ClassUnderTest.Writer.ShouldEqual(_writer);
        }
    }
}