using FubuMVC.Spark.SparkModel.Sharing;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Spark.Tests.SparkModel.Sharing
{
    [TestFixture]
    public class when_reading_imports : InteractionContext<SparkDslReader>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.ReadLine("import from Pak1", "Pak2");
            ClassUnderTest.ReadLine("import from Pak2.Design, Pak2.Bindings", "Pak2.Core");
            ClassUnderTest.ReadLine("  import from Pak3   ", "Pak4");
        }

        [Test]
        public void should_register_dependency_on_pak_for_single_provenance()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Dependency("Pak2", "Pak1"));
        }

        [Test]
        public void should_register_dependency_on_pak_for_multiple_provenances()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Dependency("Pak2.Core", "Pak2.Design"));
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Dependency("Pak2.Core", "Pak2.Bindings"));
        }

        [Test]
        public void negative_case_when_verb_is_wrong_1()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
               ClassUnderTest.ReadLine("import fromm X, Y", "Z");
            }).ShouldContainErrorMessage("Invalid verb 'fromm'");
        }

        [Test]
        public void negative_case_when_verb_is_wrong_2()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("import X, Y", "Z");
            }).ShouldContainErrorMessage("Invalid verb 'X'");
        }

        [Test]
        public void trims_the_text_before_reading_it()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Dependency("Pak4", "Pak3"));
        }

    }

    [TestFixture]
    public class when_reading_exports : InteractionContext<SparkDslReader>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.ReadLine("export to all", "Pak1");
            ClassUnderTest.ReadLine("export to Pak1", "Pak2");
            ClassUnderTest.ReadLine("export to Pak1, Pak2", "Pak3");
            ClassUnderTest.ReadLine("export to all, Pak2", "Pak3");
            ClassUnderTest.ReadLine("export to Pak2, all", "Pak3");
            ClassUnderTest.ReadLine("   export to Pak4   ", "Pak5");
        }
        
        [Test]
        public void should_register_global_when_all()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Global("Pak1"));
        }

        [Test]
        public void should_register_dependency_when_single_package()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Dependency("Pak1", "Pak2"));
        }

        [Test]
        public void should_register_dependency_when_multiple_packages()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Dependency("Pak1", "Pak3"));
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Dependency("Pak2", "Pak3"));
        }

        [Test]
        public void should_register_global_when_mixing_all_and_packages()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Global("Pak3"));
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Global("Pak3"));
        }

        [Test]
        public void negative_case_when_verb_is_wrong_1()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("export too all", "Z");
            }).ShouldContainErrorMessage("Invalid verb 'too'");
        }

        [Test]
        public void negative_case_when_verb_is_wrong_2()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("export X, Y", "Z");
            }).ShouldContainErrorMessage("Invalid verb 'X'");
        }

        [Test]
        public void trims_the_text_before_reading_it()
        {
            MockFor<ISharingRegistration>().AssertWasCalled(x => x.Dependency("Pak4", "Pak5"));
        }
    }

    [TestFixture]
    public class SparkDslReaderTester : InteractionContext<SparkDslReader>
    {
        protected override void beforeEach()
        {
            ClassUnderTest.ReadLine("#comment here", "Ozzy");
            ClassUnderTest.ReadLine(" ", "PakABC");
        }

        [Test]
        public void comments_and_empty_lines_are_ignored()
        {
            MockFor<ISharingRegistration>().AssertWasNotCalled(x => x.Dependency(Arg<string>.Is.Anything, Arg<string>.Is.Anything));
            MockFor<ISharingRegistration>().AssertWasNotCalled(x => x.Global(Arg<string>.Is.Anything));
        }

        [Test]
        public void negative_case_when_not_enough_input_1()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("export to", "Z");
            }).ShouldContainErrorMessage("Not enough tokens");            
        }

        [Test]
        public void negative_case_when_not_enough_input_2()
        {
            Exception<InvalidSyntaxException>.ShouldBeThrownBy(() =>
            {
                ClassUnderTest.ReadLine("import from", "Z");
            }).ShouldContainErrorMessage("Not enough tokens");
        }

    }
}