using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FubuMVC.Core.Util
{
    public class PropertyChain : Accessor
    {
        private readonly PropertyInfo[] _chain;
        private readonly SingleProperty _innerProperty;


        public PropertyChain(PropertyInfo[] properties)
        {
            _chain = new PropertyInfo[properties.Length - 1];
            for (int i = 0; i < _chain.Length; i++)
            {
                _chain[i] = properties[i];
            }

            _innerProperty = new SingleProperty(properties[properties.Length - 1]);
        }


        public void SetValue(object target, object propertyValue)
        {
            target = findInnerMostTarget(target);
            if (target == null)
            {
                return;
            }

            _innerProperty.SetValue(target, propertyValue);
        }

        public object GetValue(object target)
        {
            target = findInnerMostTarget(target);

            if (target == null)
            {
                return null;
            }

            return _innerProperty.GetValue(target);
        }

        public Type OwnerType { get { return _chain.Last().PropertyType; } }

        public string FieldName { get { return _innerProperty.FieldName; } }

        public Type PropertyType { get { return _innerProperty.PropertyType; } }

        public PropertyInfo InnerProperty { get { return _innerProperty.InnerProperty; } }

        public Type DeclaringType { get { return _chain[0].DeclaringType; } }

        public Accessor GetChildAccessor<T>(Expression<Func<T, object>> expression)
        {
            PropertyInfo property = ReflectionHelper.GetProperty(expression);
            var list = new List<PropertyInfo>(_chain);
            list.Add(_innerProperty.InnerProperty);
            list.Add(property);

            return new PropertyChain(list.ToArray());
        }

        public string Name
        {
            get
            {
                string returnValue = string.Empty;
                foreach (PropertyInfo info in _chain)
                {
                    returnValue += info.Name;
                }

                returnValue += _innerProperty.Name;

                return returnValue;
            }
        }


        private object findInnerMostTarget(object target)
        {
            foreach (PropertyInfo info in _chain)
            {
                target = info.GetValue(target, null);
                if (target == null)
                {
                    return null;
                }
            }

            return target;
        }

        public override string ToString()
        {
            return _chain.First().DeclaringType.FullName + _chain.Select(x => x.Name).Join(".");
        }

        public bool Equals(PropertyChain other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (_chain.Length != other._chain.Length) return false;

            for (int i = 0; i < _chain.Length; i++)
            {
                PropertyInfo info = _chain[i];
                PropertyInfo otherInfo = other._chain[i];

                if (!info.Equals(otherInfo)) return false;
            }

            return _innerProperty.Equals(other._innerProperty);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (PropertyChain)) return false;
            return Equals((PropertyChain) obj);
        }

        public override int GetHashCode()
        {
            return (_chain != null ? _chain.GetHashCode() : 0);
        }
    }
}