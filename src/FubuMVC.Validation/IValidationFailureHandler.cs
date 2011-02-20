namespace FubuMVC.Validation
{
    public interface IValidationFailureHandler<T>
        where T : class
    {
        void Handle();
    }
}