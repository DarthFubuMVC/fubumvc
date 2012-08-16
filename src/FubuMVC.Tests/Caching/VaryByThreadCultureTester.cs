using System.Collections.Generic;
using System.Threading;
using FubuMVC.Core.Caching;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Tests.Caching
{
    [TestFixture]
    public class VaryByThreadCultureTester
    {
        [Test]
        public void returns_just_the_Thread_CurrentUICulture()
        {
            var pair = new VaryByThreadCulture().Values().Single();
            pair.Key.ShouldEqual("culture");

            pair.Value.ShouldEqual(Thread.CurrentThread.CurrentUICulture.Name);
        }
    }
}