using Bottles.Diagnostics;
using Bottles.Exploding;
using Fubu;
using FubuMVC.Core.Packaging;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace fubu.Testing
{
    public class FakePackageLog : PackageLog
    {
        public bool Equals(FakePackageLog other)
        {
            return !ReferenceEquals(null, other);
        }

        public override bool Equals(object obj)
        {
            if (obj is IPackageLog) return true;

            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FakePackageLog)) return false;
            return Equals((FakePackageLog) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    [TestFixture]
    public class PackagesCommandTester : InteractionContext<PackagesCommand>
    {
        private PackagesInput theInput;

        protected override void beforeEach()
        {
            theInput = new PackagesInput(){
                AppFolder = ".\\packagesCommand"
            };

            Services.PartialMockTheClassUnderTest();

            ClassUnderTest.Stub(x => x.BuildExploder()).Return(MockFor<IBottleExploder>());
        }

        private void execute()
        {
            ClassUnderTest.Execute(theInput, MockFor<IPackageService>());
        }

        [Test]
        public void execute_with_the_explode_flag()
        {
            theInput.ExplodeFlag = true;

            execute();

            MockFor<IBottleExploder>().AssertWasCalled(x => x.ExplodeAllZipsAndReturnPackageDirectories(theInput.AppFolder, new FakePackageLog()));
        }

        [Test]
        public void execute_with_the_explode_flag_set_to_false_should_not_explode()
        {
            theInput.ExplodeFlag = false;

            execute();

            MockFor<IBottleExploder>().AssertWasNotCalled(x => x.ExplodeAllZipsAndReturnPackageDirectories(theInput.AppFolder, new FakePackageLog()));
        }

        [Test]
        public void execute_with_the_clean_all_flag()
        {
            theInput.CleanAllFlag = true;

            execute();

            MockFor<IPackageService>().AssertWasCalled(x => x.CleanAllPackages(theInput.AppFolder));
        }


        [Test]
        public void execute_with_the_clean_all_flag_set_to_false_does_not_clean()
        {
            theInput.CleanAllFlag = false;

            execute();

            MockFor<IBottleExploder>().AssertWasNotCalled(x => x.CleanAll(theInput.AppFolder));
        }
    }
}