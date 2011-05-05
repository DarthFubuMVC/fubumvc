using System;
using System.IO;
using FubuMVC.Spark.Rendering;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Spark.Tests.Rendering
{
    [TestFixture]
    public class NestedOutputTester : InteractionContext<NestedOutput>
    {
        [Test]
        public void if_the_writer_has_not_been_set_is_active_returns_false()
        {
            ClassUnderTest.IsActive().ShouldBeFalse();
        }

        [Test]
        public void if_the_writer_has_been_set_is_active_returns_true()
        {
            ClassUnderTest.SetWriter(() => new StringWriter());
            ClassUnderTest.IsActive().ShouldBeTrue();
        }

        [Test]
        public void writer_executes_the_delegate_passed_on_set_writer()
        {
            TextWriter writer = new StringWriter();
            Func<TextWriter> writerFunc = () => writer;
            ClassUnderTest.SetWriter(writerFunc);
            ClassUnderTest.Writer.ShouldEqual(writer);
        }
    }
}