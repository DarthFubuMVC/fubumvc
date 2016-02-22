using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Validation
{
    public class Validator : IValidator
    {
        private readonly ITypeResolver _typeResolver;
        private readonly ValidationGraph _graph;
        private readonly IServiceLocator _services;

        public Validator(ITypeResolver typeResolver, ValidationGraph graph, IServiceLocator services)
        {
            _typeResolver = typeResolver;
            _graph = graph;
            _services = services;
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
            var context = ContextFor(target, notification);

            _graph.PlanFor(validatedType).Execute(context);
        }

        public ValidationContext ContextFor(object target, Notification notification)
        {
            var validatedType = _typeResolver.ResolveType(target);
            return new ValidationContext(this, notification, target)
            {
                TargetType = validatedType,
                Resolver = _typeResolver,
                ServiceLocator = _services
            };
        }

        public static IValidator BasicValidator()
        {
            return new Validator(new TypeResolver(), ValidationGraph.BasicGraph(), new InMemoryServiceLocator());
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