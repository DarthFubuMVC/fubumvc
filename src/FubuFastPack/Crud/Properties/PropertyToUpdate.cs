using System;
using System.Reflection;
using FubuCore.Reflection;

namespace FubuFastPack.Crud.Properties
{
    public class PropertyToUpdate
    {
        public PropertyInfo Property { get; set; }
        public Type EntityType { get; set; }

        public static PropertyToUpdate For<T>(Accessor accessor)
        {
            return new PropertyToUpdate
                   {
                       EntityType = typeof(T),
                       Property = accessor.InnerProperty
                   };
        }
    }
}