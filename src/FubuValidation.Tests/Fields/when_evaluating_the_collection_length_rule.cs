using NUnit.Framework;

namespace FubuValidation.Tests.Fields
{
    /*
        public static readonly string FIELD = "field";
        public static readonly string LENGTH = "length";

        public int? Length { get; set; }

        public NameValueCollection GetMessageSubstitutions(Accessor accessor)
        {
            return new NameValueCollection
                       {
                           {FIELD, accessor.Name},
                           {LENGTH, Length.GetValueOrDefault(-1).ToString()}
                       };
        }

        public ValidationStrategyResult Validate(ValidationStrategyContext context)
        {
            var rawValue = context.RawValue;
            var enumerable = rawValue as IEnumerable;
            if(enumerable == null || (Length.HasValue && enumerable.Count() != Length.Value))
            {
                return ValidationStrategyResult.Invalid(new NotificationMessage(ValidationKeys.COLLECTION_LENGTH));
            }

            return ValidationStrategyResult.Valid();
        }
     */


    [TestFixture]
    public class when_evaluating_the_collection_length_rule
    {

    }
}