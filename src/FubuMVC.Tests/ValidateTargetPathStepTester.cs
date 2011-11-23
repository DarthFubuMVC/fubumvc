using Fubu;
using FubuCore;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class ValidateTargetPathStepTester : InteractionContext<ValidateTargetPathStep>
    {
        [Test]
        public void should_throw_fubu_exception_if_target_path_already_exists()
        {
            var context = new TemplatePlanContext { TargetPath = "Test" };
            MockFor<IFileSystem>()
                .Expect(s => s.DirectoryExists(context.TargetPath))
                .Return(true);

            Exception<FubuException>.ShouldBeThrownBy(() => ClassUnderTest.Execute(context));
        }

        [Test]
        public void should_not_throw_exception_of_target_path_does_not_exist()
        {
            var context = new TemplatePlanContext { TargetPath = "Test" };
            MockFor<IFileSystem>()
                .Expect(s => s.DirectoryExists(context.TargetPath))
                .Return(false);

            ClassUnderTest.Execute(context);
        }
    }
}