using System;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Models
{
    public class StandardModelBinder : IModelBinder
    {
        private readonly IValueConverterRegistry _converters;
        private readonly ITypeDescriptorRegistry _typeRegistry;

        public StandardModelBinder(IValueConverterRegistry converters, ITypeDescriptorRegistry typeRegistry)
        {
            _converters = converters;
            _typeRegistry = typeRegistry;
        }

        public bool Matches(Type type)
        {
            return type.GetConstructors().Count(x => x.GetParameters().Length == 0) == 1;
        }

        public BindResult Bind(Type type, IBindingContext context)
        {
            return Bind(type, Activator.CreateInstance(type), context);
        }

        public BindResult Bind(Type type, object instance, IBindingContext context)
        {
            var result = new BindResult
            {
                Value = instance
            };

            Populate(result, type, context);

            return result;
        }


        // Only exists for easier testing
        public void Populate(object target, IBindingContext context)
        {
            Populate(new BindResult
            {
                Value = target
            }, target.GetType(), context);
        }

        private void Populate(BindResult result, Type type, IBindingContext context)
        {
            _typeRegistry.ForEachProperty(type, prop =>
            {
// ReSharper disable ConvertToLambdaExpression
                context.Value(prop, o =>
                {
                    try
                    {
                        var converter = _converters.FindConverter(prop);
                        var value = converter(new RawValue()
                        {
                            Context = context,
                            Property = prop,
                            Value = o
                        });

                        prop.SetValue(result.Value, value, null);
                    }
                    catch (Exception e)
                    {
                        var problem = new ConvertProblem
                        {
                            Exception = e,
                            Item = result.Value,
                            Property = prop,
                            Value = o
                        };

                        result.Problems.Add(problem);
                    }
                });
// ReSharper restore ConvertToLambdaExpression
            });
        }

    }
}