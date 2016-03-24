namespace FubuMVC.Core.Validation.Web
{
    public interface IValidationFilter<T>
    {
        Notification Validate(T input);
    }

    public class ValidationFilter<T> : IValidationFilter<T>
    {
        private readonly IValidator _validator;
        private readonly IModelBindingErrors _errors;

        public ValidationFilter(IValidator validator, IModelBindingErrors errors)
        {
            _validator = validator;
            _errors = errors;
        }

        public Notification Validate(T input)
        {
            var notification = _validator.Validate(input);
            
            _errors.AddAnyErrors<T>(notification);

            return notification;
        }
    }
}