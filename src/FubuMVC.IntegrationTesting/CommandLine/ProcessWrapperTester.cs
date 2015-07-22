using System.Diagnostics;
using Fubu;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.CommandLine
{
    [TestFixture]
    public class ProcessWrapperTester
    {
        [Test] 
        public void should_have_same_ProcessStartInfo()
        {
            var ClassUnderTest = new ProcessWrapper(new ProcessStartInfo());
            Assert.AreEqual(ClassUnderTest.ProcessStartInfo, ClassUnderTest.WrappedProcess.StartInfo);
        }
    }
}