using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Views.Razor
{
    [TestFixture]
    public class Handles_null_properties : ViewIntegrationContext
    {
        public Handles_null_properties()
        {
            RazorView<ComplexModelViewModel>("ComplexModel").Write(@"
<p>ComplexModel.cshtml</p>
This is just a test to see if we can set properties that are null.
<p>@this.Model.TestString</p><p>@this.Model.TestStringThatIsNull</p>
");
        }

        [Test]
        public void handles_null_just_fine()
        {
            Scenario.Get.Action<ComplexModelEndpoint>(e => e.get_razor_complex());
            Scenario.ContentShouldContain("<p>Test String</p><p></p>");
        }
    }

    public class ComplexModelEndpoint
    {
        public ComplexModelViewModel get_razor_complex()
        {
            return new ComplexModelViewModel { TestString = "Test String" };
        }
    }

    public class ComplexModelViewModel
    {
        public string TestString { get; set; }
        public string TestStringThatIsNull { get; set; }
    }
}