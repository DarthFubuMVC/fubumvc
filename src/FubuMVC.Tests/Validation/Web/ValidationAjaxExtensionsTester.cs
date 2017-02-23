using FubuMVC.Core.Ajax;
using FubuMVC.Core.Validation.Web;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
	
	public class ValidationAjaxExtensionsTester
	{
		[Fact]
		public void gets_and_sets_the_validation_origin()
		{
			var continuation = new AjaxContinuation();

			continuation.ValidationOrigin(ValidationOrigin.Server);
			continuation.ValidationOrigin().ShouldBe(ValidationOrigin.Server);

			continuation.ValidationOrigin(ValidationOrigin.Client);
			continuation.ValidationOrigin().ShouldBe(ValidationOrigin.Client);
		}
	}
}