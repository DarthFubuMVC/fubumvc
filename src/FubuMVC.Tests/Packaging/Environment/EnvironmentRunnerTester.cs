using FubuMVC.Core.Packaging.Environment;
using NUnit.Framework;

namespace FubuMVC.Tests.Packaging.Environment
{
    [TestFixture]
    public class EnvironmentRunnerTester
    {
        [Test]
        public void environment_runner_should_find_the_specific_class_if_it_is_specified()
        {
            var run = new EnvironmentRun(){
                EnvironmentClassName = typeof (FakeEnvironment).AssemblyQualifiedName
            };

            var runner = new EnvironmentRunner(run);

            runner.FindEnvironment().ShouldBeOfType<FakeEnvironment>();
        }

        [Test]
        public void environment_runner_can_scan_an_assembly_to_find_an_environment_if_a_specific_one_is_not_given()
        {
            var run = new EnvironmentRun(){
                AssemblyName = GetType().Assembly.FullName
            };

            var runner = new EnvironmentRunner(run);

            runner.FindEnvironment().ShouldBeOfType<FakeEnvironment>();
        }
    }
}