using System;
using System.Collections.Generic;
using FubuMVC.Core.Packaging;
using NUnit.Framework;

namespace FubuMVC.Tests.Packaging
{
    [TestFixture]
    public class PackageRegistryTester
    {
        [Test]
        public void assert_failures_with_no_failures()
        {
            PackageRegistry.LoadPackages(x =>
            {
                x.Bootstrap(log =>
                {
                    return new List<IActivator>();
                });
            });

            PackageRegistry.AssertNoFailures();
        }

        [Test]
        public void assert_failures_blows_up_when_anything_in_the_diagnostics_has_a_problem()
        {
            PackageRegistry.LoadPackages(x =>
            {
                x.Bootstrap(log =>
                {
                    throw new ApplicationException("You shall not pass");
                });
            });

            Exception<ApplicationException>.ShouldBeThrownBy(() =>
            {
                PackageRegistry.AssertNoFailures();
            }).Message.ShouldContain("You shall not pass");
        }
    }
}