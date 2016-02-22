using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuMVC.Core.Validation.Fields;

namespace FubuMVC.Core.Validation
{
    public class ValidationPlan : DescribesItself
    {
        private readonly Type _type;
        private readonly IEnumerable<ValidationStep> _steps;
        private readonly Lazy<ClassFieldValidationRules> _fieldRules;

        public ValidationPlan(Type type, IEnumerable<ValidationStep> steps)
        {
            _type = type;
            _steps = steps;

            _fieldRules = new Lazy<ClassFieldValidationRules>(() => _steps.SelectMany(step => step.Rules).OfType<ClassFieldValidationRules>().SingleOrDefault());
        }

        public IEnumerable<ValidationStep> Steps { get { return _steps; }} 

        public virtual void Execute(ValidationContext context)
        {
            _steps.Each(step => step.Execute(context));
        }

        public ClassFieldValidationRules FieldRules
        {
            get { return _fieldRules.Value ?? new ClassFieldValidationRules(); }
        }

		public IEnumerable<T> FindRules<T>()
			where T : IValidationRule
		{
			return _steps.SelectMany(x => x.FindRules<T>());
		}

        public void Describe(Description description)
        {
            var list = description.AddList("ValidationSteps", _steps);
            list.Label = "Validation Steps";
            list.IsOrderDependent = true;
        }

        public override string ToString()
        {
            return "Validate {0}".ToFormat(_type.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ValidationPlan)) return false;
            return Equals((ValidationPlan) obj);
        }

        public bool Equals(ValidationPlan other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._type == _type && _steps.SequenceEqual(other._steps);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_type.GetHashCode()*397) ^ _steps.GetHashCode();
            }
        }

		public static ValidationPlan For(Type type, ValidationGraph graph)
		{
			var steps = new List<ValidationStep>();
			graph.Sources.Each(source =>
			{
				var rules = source.RulesFor(type);
				if (!rules.Any()) return;

				steps.Add(new ValidationStep(type, source.GetType(), rules));
			});

			return new ValidationPlan(type, steps);
		}
    }
}