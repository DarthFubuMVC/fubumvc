namespace FubuValidation
{
    public class ValidationStrategyResult
    {
        private ValidationStrategyResult() { }

        public bool IsValid { get; private set; }
        public NotificationMessage Message { get; private set; }

        public static ValidationStrategyResult Valid()
        {
            return new ValidationStrategyResult
                       {
                           IsValid = true
                       };
        }

        public static ValidationStrategyResult Invalid(NotificationMessage message)
        {
            return new ValidationStrategyResult
                       {
                           IsValid = false,
                           Message = message
                       };
        }
    }
}