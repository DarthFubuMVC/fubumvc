namespace FubuValidation
{
    public class ValidationContext
    {
        private readonly IValidationProvider _provider;

        public ValidationContext(IValidationProvider provider)
        {
            _provider = provider;
        }

        public IValidationProvider Provider
        {
            get { return _provider; }
        }
    }
}