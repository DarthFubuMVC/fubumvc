using System;
using System.Linq.Expressions;
using FubuCore.Reflection;

namespace Bottles.Deployment.Writing
{
    public class PropertyValue
    {
        public Accessor Accessor { get; set; }
        public object Value { get; set; }
        
        public static PropertyValue For<T>(Expression<Func<T, object>> expression, object value)
        {
            return new PropertyValue(){
                Accessor = expression.ToAccessor(),
                Value = value
            };
        }
    }
}