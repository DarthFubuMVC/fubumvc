using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuValidation.Fields;

namespace FubuValidation
{
    public class Validator : IValidator
    {
        private readonly IList<IValidationSource> _sources;
        private readonly ITypeResolver _typeResolver;

        public Validator(ITypeResolver typeResolver, IEnumerable<IValidationSource> sources)
        {
            _typeResolver = typeResolver;
            _sources = new List<IValidationSource>(sources){
                new SelfValidatingClassRuleSource()
            };
        }

        public Notification Validate(object target)
        {
            var validatedType = _typeResolver.ResolveType(target);
            var notification = new Notification(validatedType);
            Validate(target, notification);
            return notification;
        }

        public void Validate(object target, Notification notification)
        {
            var validatedType = _typeResolver.ResolveType(target);
            var context = new ValidationContext(this, notification, target){
                TargetType = validatedType,
                Resolver = _typeResolver
            };

            _sources.SelectMany(x => x.RulesFor(validatedType))
                .Each(rule => rule.Validate(context));
        }

        public static IValidator BasicValidator()
        {
            return new Validator(new TypeResolver(), new IValidationSource[]{
                new FieldRuleSource(new FieldRulesRegistry(new IFieldValidationSource[0], new TypeDescriptorCache()))
            });
        }

        public static Notification ValidateObject(object target)
        {
            return BasicValidator().Validate(target);
        }

        public static IEnumerable<NotificationMessage> ValidateField(object target, string propertyName)
        {
            var notification = ValidateObject(target);
            return notification.AllMessages.Where(x => x.Accessors.Any(a => a.Name == propertyName));
        }
            
    }
}