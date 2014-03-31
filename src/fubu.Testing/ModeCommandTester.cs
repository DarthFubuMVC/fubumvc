using Fubu;
using FubuMVC.Core;
using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;

namespace fubu.Testing
{
    [TestFixture, Explicit("It's SLOW")]
    public class ModeCommandTester
    {
        private string originalMode;
        private ModeCommand theCommand;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            originalMode = FubuMode.Mode();
            theCommand = new ModeCommand();
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            FubuMode.Detector.SetMode(originalMode);
        }

        [Test]
        public void clear_the_mode()
        {
            theCommand.Execute(new ModeInput{
                DevFlag = true
            });

            theCommand.Execute(new ModeInput{
                ClearFlag = true
            });

            FubuMode.Reset();
            FubuMode.Mode().IsEmpty().ShouldBeTrue();
        }

        [Test]
        public void set_to_dev_mode()
        {
            theCommand.Execute(new ModeInput
            {
                ClearFlag = true
            });

            theCommand.Execute(new ModeInput
            {
                DevFlag = true
            });

            FubuMode.Reset();

            FubuMode.InDevelopment().ShouldBeTrue();
        }

        [Test]
        public void set_to_another_mode()
        {
            theCommand.Execute(new ModeInput{
                ModeFlag = "Something"
            });

            FubuMode.Reset();

            FubuMode.Mode().ShouldEqual("Something");
        }
    }
}