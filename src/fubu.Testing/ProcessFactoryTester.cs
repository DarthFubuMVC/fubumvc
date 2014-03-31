using System;
using Fubu;
using FubuTestingSupport;
using NUnit.Framework;

namespace fubu.Testing
{
    [TestFixture]
    public class ProcessFactoryTester : InteractionContext<ProcessFactory>
    {
        [Test] 
        public void should_throw_ArgumentNullException_when_passed_null()
        {
            Assert.Throws<ArgumentNullException>(() => ClassUnderTest.Create(null));
        }

        [Test]
        public void should_create_IProcess()
        {
            var result = ClassUnderTest.Create(p => { });
            Assert.NotNull(result);
        }

        [Test]
        public void should_invoke_configuration_action()
        {
            var hit = false;
            ClassUnderTest.Create(p => hit = true);
            Assert.True(hit);
        }
    }
}