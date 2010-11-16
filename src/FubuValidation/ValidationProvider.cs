using System.Collections.Generic;
using FubuCore;
using FubuValidation.Registration;

namespace FubuValidation
{
    public class ValidationProvider : IValidationProvider
    {
        private readonly ITypeResolver _typeResolver;
        private readonly IValidationQuery _query;

        public ValidationProvider(ITypeResolver typeResolver, IValidationQuery query)
        {
            _typeResolver = typeResolver;
            _query = query;
        }

        public Notification Validate(object target)
        {
            var notification = new Notification(_typeResolver.ResolveType(target));
            Validate(target, notification);
            return notification;
        }

        public void Validate(object target, Notification notification)
        {
            _query
                .RulesFor(target)
                .Each(rule => rule.Validate(target, notification));
        }
    }
}