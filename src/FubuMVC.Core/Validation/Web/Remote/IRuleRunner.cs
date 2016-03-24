namespace FubuMVC.Core.Validation.Web.Remote
{
    public interface IRuleRunner
    {
        Notification Run(RemoteFieldRule remote, string value);
    }

    public class RuleRunner : IRuleRunner
    {
        private readonly IValidator _validator;
        private readonly IValidationTargetResolver _resolver;

        public RuleRunner(IValidator validator, IValidationTargetResolver resolver)
        {
            _validator = validator;
            _resolver = resolver;
        }

        public Notification Run(RemoteFieldRule remote, string value)
        {
            var target = _resolver.Resolve(remote.Accessor, value);
            var context = _validator.ContextFor(target, new Notification(target.GetType()));

            remote.Rule.Validate(remote.Accessor, context);

            return context.Notification;
        }
    }
}