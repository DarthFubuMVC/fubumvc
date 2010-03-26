using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Binding;
using Microsoft.Practices.ServiceLocation;

namespace FubuCore.Binding
{
    public class BindingContext : IBindingContext
    {
        private static readonly List<Func<PropertyInfo, string>> _namingStrategies;
        private readonly IServiceLocator _locator;
        private readonly IRequestData _requestData;
        private readonly IList<ConvertProblem> _problems = new List<ConvertProblem>();

        static BindingContext()
        {
            _namingStrategies = new List<Func<PropertyInfo, string>>
            {
                p => p.Name,
                p => p.Name.Replace("_", "-")
            };
        }

        public BindingContext(IRequestData requestData, IServiceLocator locator)
        {
            _requestData = requestData;
            _locator = locator;
        }

        public IList<ConvertProblem> Problems
        {
            get { return _problems; } }

        public T Service<T>()
        {
            return _locator.GetInstance<T>();
        }

        public object PropertyValue { get; protected set; }
        
        private readonly Stack<PropertyInfo> _propertyStack = new Stack<PropertyInfo>();
        public PropertyInfo Property
        {
            get { return _propertyStack.Peek(); }
        }

        private readonly Stack<object> _objectStack = new Stack<object>();
        public void ForProperty(PropertyInfo property, Action action)
        {
            _propertyStack.Push(property);

            try
            {
                findPropertyValueInRequestData(action);
            }
            catch (Exception ex)
            {
                LogProblem(ex);
            }
            finally
            {
                _propertyStack.Pop();
            }
        }

        private void findPropertyValueInRequestData(Action action)
        {
            _namingStrategies.Any(naming =>
            {
                string name = naming(Property);
                return _requestData.Value(name, o =>
                {
                    PropertyValue = o;
                    action();
                });
            });
        }

        public object Object
        {
            get { return _objectStack.Any() ? _objectStack.Peek() : null; } }

        public void StartObject(object @object)
        {
            _objectStack.Push(@object);
        }

        public void FinishObject()
        {
            _objectStack.Pop();
        }


        public IBindingContext PrefixWith(string prefix)
        {
            return prefixWith(prefix, _propertyStack.Reverse());
        }

        private BindingContext prefixWith(string prefix, IEnumerable<PropertyInfo> properties)
        {
            var prefixedData = new PrefixedRequestData(_requestData, prefix);
            var child = new BindingContext(prefixedData, _locator);
            properties.Each(p => child._propertyStack.Push(p));
            return child;
        }

        public void LogProblem(Exception ex)
        {
            LogProblem(ex.ToString());
        }

        public void LogProblem(string exceptionText)
        {
            var problem = new ConvertProblem()
            {
                ExceptionText = exceptionText,
                Item = Object,
                Properties = _propertyStack.ToArray().Reverse(),
                Value = PropertyValue
            };

            _problems.Add(problem);
        }

        public void BindChild(PropertyInfo property, Type childType, string prefix)
        {
            var target = Object;
            _propertyStack.Push(property);

            var resolver = Service<IObjectResolver>();
            var context = prefixWith(prefix, _propertyStack.Reverse());

            try
            {
                var result = resolver.BindModel(childType, context);
                property.SetValue(target, result.Value, null);
            }
            catch (Exception e)
            {
                LogProblem(e);
            }

            _problems.AddRange(context._problems);

            _propertyStack.Pop();
        }

        public void BindChild(PropertyInfo property)
        {
            BindChild(property, property.PropertyType, property.Name);
        }
    }
}