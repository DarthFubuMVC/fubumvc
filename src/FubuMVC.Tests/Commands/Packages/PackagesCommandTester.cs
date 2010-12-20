using System;
using Fubu.Packages;
using FubuCore;
using FubuMVC.Core.Packaging;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Commands.Packages
{
    [TestFixture]
    public class PackagesCommandTester : InteractionContext<PackagesCommand>
    {
        private PackagesInput theInput;

        protected override void beforeEach()
        {
            theInput = new PackagesInput(){
                AppFolder = "c:\\folder1"
            };
            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.Stub(x => x.BuildExploder()).Return(MockFor<IPackageExploder>());
        }

        private void execute()
        {
            ClassUnderTest.Execute(theInput);
        }

        [Test]
        public void execute_with_the_explode_flag()
        {
            theInput.ExplodeFlag = true;

            execute();

            MockFor<IPackageExploder>().AssertWasCalled(x => x.ExplodeAll(theInput.AppFolder));
        }

        [Test]
        public void execute_with_the_explode_flag_set_to_false_should_not_explode()
        {
            theInput.ExplodeFlag = false;

            execute();

            MockFor<IPackageExploder>().AssertWasNotCalled(x => x.ExplodeAll(theInput.AppFolder));
        }

        [Test]
        public void execute_with_the_clean_all_flag()
        {
            theInput.CleanAllFlag = true;

            execute();

            MockFor<IPackageExploder>().AssertWasCalled(x => x.CleanAll(theInput.AppFolder));
        }


        [Test]
        public void execute_with_the_clean_all_flag_set_to_false_does_not_clean()
        {
            theInput.CleanAllFlag = false;

            execute();

            MockFor<IPackageExploder>().AssertWasNotCalled(x => x.CleanAll(theInput.AppFolder));
        }
    }
}