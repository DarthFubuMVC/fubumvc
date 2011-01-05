using System;
using FubuMVC.Core.UI.Scripts;
using NUnit.Framework;

namespace FubuMVC.Tests.UI.Scripts
{
    [TestFixture]
    public class ScriptDependencyGraphTester
    {

        [Test]
        public void TESTNAME()
        {
            "a".CompareTo("b").ShouldEqual(-1);
        }

    }

    public class StubScriptGraphLogger : IScriptGraphLogger
    {
        
    }
}