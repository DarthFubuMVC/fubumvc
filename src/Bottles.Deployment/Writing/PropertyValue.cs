using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;

namespace Bottles.Deployment.Writing
{
    public class PropertyValue
    {
        public Accessor Accessor { get; set; }
        public object Value { get; set; }

        public string HostName { get; set; }
        
        public static PropertyValue For<T>(Expression<Func<T, object>> expression, object value)
        {
            return new PropertyValue(){
                Accessor = expression.ToAccessor(),
                Value = value
            };
        }

        public override string ToString()
        {
            var description = "{0}.{1}={2}".ToFormat(
                Accessor.DeclaringType.Name, 
                Accessor.PropertyNames.Join("."), 
                Value == null ? string.Empty : Value.ToString());

            return HostName.IsEmpty() ? description : "{0}.{1}".ToFormat(HostName, description);
        }
    }
}