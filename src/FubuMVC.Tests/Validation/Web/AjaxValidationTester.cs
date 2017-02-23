using FubuMVC.Core.Ajax;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Web;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.Validation.Web
{
    
    public class AjaxValidationTester
    {
        private Notification theNotification;

        public AjaxValidationTester()
        {
			LocalizationManager.Stub();

            theNotification = new Notification(typeof(SampleInputModel));
            theNotification.RegisterMessage<SampleInputModel>(m => m.Field, StringToken.FromKeyString("Field", "Message"));
        }


        [Fact]
        public void should_add_corresponding_errors()
        {
            theContinuation
                .Errors
                .ShouldHaveCount(1);
        }

        [Fact]
        public void should_set_field_for_errors()
        {
            theContinuation
                .Errors
                .ShouldContain(e => e.field.Equals("Field"));
        }

		[Fact]
		public void sets_the_label()
		{
			theContinuation
				.Errors
				.ShouldContain(e => e.label.Equals("en-US_Field"));
		}

        [Fact]
        public void should_set_messages_for_errors()
        {
            theContinuation
                .Errors
                .ShouldContain(e => e.message.Equals("Message"));
        }

        [Fact]
        public void should_set_success_flag()
        {
            theContinuation
                .Success
                .ShouldBeFalse();

            theNotification = new Notification(typeof(SampleInputModel));

            theContinuation
                .Success
                .ShouldBeTrue();
        }

        private AjaxContinuation theContinuation
        {
            get { return AjaxValidation.BuildContinuation(theNotification); }
        }
    }
}