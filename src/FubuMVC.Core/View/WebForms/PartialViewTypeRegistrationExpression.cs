namespace FubuMVC.Core.View.WebForms
{
    public class PartialViewTypeRegistrationExpression : IPartialViewTypeRegistrationExpression
    {
        private readonly IPartialViewTypeRegistry _registry;

        public PartialViewTypeRegistrationExpression(IPartialViewTypeRegistry registry)
        {
            _registry = registry;
        }

        public IPartialViewTypeBindingExpression For<TPartialModel>()
        {
            return new PartialViewTypeBindingExpression(_registry, typeof(TPartialModel));
        }
    }
}