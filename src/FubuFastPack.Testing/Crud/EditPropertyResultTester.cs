using FubuCore.Reflection;
using FubuFastPack.Crud.Properties;
using FubuFastPack.Testing.Security;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuFastPack.Testing.Crud
{
    [TestFixture]
    public class EditPropertyResultTester
    {
        private readonly Accessor _accessor = ReflectionHelper.GetAccessor<Case>(c => c.CaseType);

        [Test]
        public void ListName_should_be_set_if_the_accessor_is_a_list_type()
        {
            var result = new EditPropertyResult(_accessor, typeof(Case));
            result.ListName.ShouldEqual("CaseType");
        }

        [Test]
        public void IsListAccessor_should_be_true_if_the_accessor_is_a_list_type()
        {
            var result = new EditPropertyResult(_accessor, typeof(Case));
            result.IsListAccessor().ShouldBeTrue();
        }

        [Test]
        public void Failure_should_set_WasNotApplied()
        {
            var result = EditPropertyResult.Failure(_accessor, typeof(Case), "unused");
            result.WasNotApplied.ShouldBeTrue();
        }

        [Test]
        public void Failure_should_set_failure_message()
        {
            const string expectedFailureMessage = "expected failure message";
            var result = EditPropertyResult.Failure(_accessor, typeof(Case), expectedFailureMessage);
            result.FailureMessage.ShouldEqual(expectedFailureMessage);
        }
    }
}