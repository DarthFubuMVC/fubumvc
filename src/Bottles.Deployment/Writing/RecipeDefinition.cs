using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace Bottles.Deployment.Writing
{
    public class RecipeDefinition
    {
        private readonly IList<IDirective> _directives = new List<IDirective>();
        private readonly IList<BottleReference> _references = new List<BottleReference>();
        private readonly IList<PropertyValue> _values = new List<PropertyValue>();

        public void AddReference(BottleReference reference)
        {
            _references.Add(reference);
        }

        public void AddDirectives(IEnumerable<IDirective> directives)
        {
            _directives.AddRange(directives);
        }

        public void AddDirective(IDirective directive)
        {
            _directives.Add(directive);
        }

        public void AddProperty<T>(Expression<Func<T, object>> expression, object value)
        {
            _values.Add(new PropertyValue(){
                Accessor = expression.ToAccessor(),
                Value = value
            });
        }
    }
}