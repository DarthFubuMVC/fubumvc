using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Forms;

namespace FubuMVC.Core.Validation.Web.UI
{
    public class ValidationOptions
    {
        public const string Data = "validation-options";

        private int _elementTimeout = ValidationNode.DefaultTimeout;
        private readonly IList<FieldOptions> _fields = new List<FieldOptions>();

        public FieldOptions[] fields { get { return _fields.ToArray(); } }

        public int elementTimeout { get { return _elementTimeout; } }

        protected bool Equals(ValidationOptions other)
        {
            return _fields.SequenceEqual(other._fields);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValidationOptions)obj);
        }

        public override int GetHashCode()
        {
            return _fields.GetHashCode();
        }

        public static ValidationOptions For(FormRequest request)
        {
            var services = request.Services;
            var type = services.GetInstance<ITypeResolver>().ResolveType(request.Input);
            var cache = services.GetInstance<ITypeDescriptorCache>();
            var options = new ValidationOptions();
            var node = request.Chain.ValidationNode();

            if (node == null)
            {
                return options;
            }

            options._elementTimeout = node.ElementTimeout;

            cache.ForEachProperty(type, property =>
            {
                var accessor = new SingleProperty(property);

                fillFields(options, node, services, accessor, request.Input == null ? property.DeclaringType : request.Input.GetType());
            });

            return options;
        }

        private static void fillFields(ValidationOptions options, IValidationNode node, IServiceLocator services, Accessor accessor, Type type)
        {
            var mode = node.DetermineMode(services, accessor);
            var field = new FieldOptions
            {
                field = accessor.Name,
                mode = mode.Mode
            };

            var graph = services.GetInstance<ValidationGraph>();
            var rules = graph.FieldRulesFor(type, accessor);
            var ruleOptions = new List<FieldRuleOptions>();

            rules.Each(rule =>
            {
                var ruleMode = rule.Mode ?? mode;
                ruleOptions.Add(new FieldRuleOptions
                {
                    rule = RuleAliases.AliasFor(rule),
                    mode = ruleMode.Mode
                });
            });

            field.rules = ruleOptions.ToArray();

            options._fields.Add(field);
        }
    }
}