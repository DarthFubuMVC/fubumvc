using FubuMVC.Core.Packaging.Environment;
using NUnit.Framework;

namespace FubuMVC.Tests.Packaging.Environment
{
    [TestFixture]
    public class EnvironmentRunTester 
    {
        [Test]
        public void find_specific_class_if_environment_class_is_specified()
        {
            var run = new EnvironmentRun()
                      {
                          EnvironmentClassName = typeof(FakeEnvironment).AssemblyQualifiedName
                      };

            run.FindEnvironmentType().ShouldEqual(typeof (FakeEnvironment));
        }

        [Test]
        public void find_environment_class_by_scanning_assembly_if_no_class_is_specified()
        {
            var run = new EnvironmentRun()
                      {
                          EnvironmentClassName = typeof(FakeEnvironment).AssemblyQualifiedName
                      };

            run.FindEnvironmentType().ShouldEqual(typeof(FakeEnvironment));
        }

        [Test]
        public void build_app_domain_setup()
        {
            var run = new EnvironmentRun(){
                ApplicationBase = "c:\\folder1",
                ConfigurationFile = "web.config"
            };

            var setup = run.BuildAppDomainSetup();
            setup.ApplicationBase.ShouldEqual(run.ApplicationBase);
            setup.ConfigurationFile.ShouldEqual("c:\\folder1\\web.config");
            setup.ApplicationName.ShouldStartWith("Bottles-Environment-Installation");
            setup.ShadowCopyFiles.ShouldEqual("true");
        }

        [Test]
        public void assert_is_valid_with_application_base_and_assembly_name_set_should_not_throw()
        {
            var run = new EnvironmentRun()
                      {
                          ApplicationBase = ".",
                          AssemblyName = "SomeAssembly"
                      };

            run.AssertIsValid();
        }

        [Test]
        public void assert_is_valid_with_application_base_and_environment_class_name_but_not_assembly()
        {
            var run = new EnvironmentRun()
                      {
                          ApplicationBase = ".",
                          EnvironmentClassName = "some class"
                      };

            run.AssertIsValid();
        }

        [Test]
        public void assert_is_valid_missing_application_base_throws()
        {
            var run = new EnvironmentRun{
                EnvironmentClassName = "some class"
            };

            Exception<EnvironmentRunnerException>.ShouldBeThrownBy(() =>
            {
                run.AssertIsValid();
            }).Message.ShouldContain("ApplicationBase must be a valid folder");
        }

        [Test]
        public void assert_is_valid_missing_both_environment_class_name_and_assembly_name_throws()
        {
            var run = new EnvironmentRun(){
                ApplicationBase = "c:\\folder1"
            };

            Exception<EnvironmentRunnerException>.ShouldBeThrownBy(() =>
            {
                run.AssertIsValid();
            }).Message.ShouldContain("Either EnvironmentClassName or AssemblyName must be set");
        }
    }
}