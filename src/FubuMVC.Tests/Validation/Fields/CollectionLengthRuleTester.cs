using System.Linq;
using FubuMVC.Core.Validation;
using FubuMVC.Core.Validation.Fields;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Validation.Fields
{
    [TestFixture]
    public class CollectionLengthRuleTester
    {
        [Test]
        public void do_not_log_any_messages_when_the_collection_is_valid()
        {
            var target = new CollectionLengthTarget(){
                Names = new[]{"a"}
            };

            var rule = new CollectionLengthRule(1);
            rule.ValidateProperty(target, x => x.Names).MessagesFor<CollectionLengthTarget>(x => x.Names).Any().ShouldBeFalse();
        }

        [Test]
        public void log_message_when_collection_is_wrong_length()
        {
            var target = new CollectionLengthTarget(){
                Names = new string[0]
            };

            var rule = new CollectionLengthRule(1);
            rule.ValidateProperty(target, x => x.Names).MessagesFor<CollectionLengthTarget>(x => x.Names).Single().GetMessage()
                .ShouldBe("Must be exactly 1 element(s)"); 
        }

        [Test]
        public void log_message_when_collection_is_null()
        {
            var target = new CollectionLengthTarget()
            {
                Names = null
            };

            var rule = new CollectionLengthRule(1);
            rule.ValidateProperty(target, x => x.Names).MessagesFor<CollectionLengthTarget>(x => x.Names).Single().GetMessage()
                .ShouldBe("Must be exactly 1 element(s)");
        }


        [Test]
        public void log_message_when_collection_is_wrong_length_w()
        {
            var target = new CollectionLengthTarget()
            {
                Names = new string[] { "a"}
            };

            var rule = new CollectionLengthRule(2);
            rule.ValidateProperty(target, x => x.Names).MessagesFor<CollectionLengthTarget>(x => x.Names).Single().GetMessage()
                .ShouldBe("Must be exactly 2 element(s)");
        }

		[Test]
		public void uses_default_collection_length_token()
		{
			new CollectionLengthRule(0).Token.ShouldBe(ValidationKeys.CollectionLength);
		}
    }
    
    public class CollectionLengthTarget
    {
        public string[] Names { get; set; }
    }

}