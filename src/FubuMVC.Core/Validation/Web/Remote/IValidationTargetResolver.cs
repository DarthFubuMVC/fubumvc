using System.Collections.Generic;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Reflection;

namespace FubuMVC.Core.Validation.Web.Remote
{
    public interface IValidationTargetResolver
    {
        object Resolve(Accessor accessor, string value);
    }

    public class ValidationTargetResolver : IValidationTargetResolver
    {
        private readonly IObjectResolver _resolver;

        public ValidationTargetResolver(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public object Resolve(Accessor accessor, string value)
        {
            var values = new Dictionary<string, string> { {accessor.Name, value} };
            return _resolver.BindModel(accessor.OwnerType, new FlatValueSource(values)).Value;
        }
    }
}