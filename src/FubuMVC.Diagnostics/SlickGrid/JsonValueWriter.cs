using System;
using System.Collections.Generic;
using FubuCore;

namespace FubuMVC.Diagnostics.SlickGrid
{
    public static class JsonValueWriter
    {
        public static object ConvertToJson(object value)
        {
            if (value is Type)
            {
                var type = value.As<Type>();
                return new Dictionary<string, object>{
                    {"Name", type.Name},
                    {"FullName", type.FullName},
                    {"Namespace", type.Namespace},
                    {"Assembly", type.Assembly.FullName}
                };
            }

            return value;
        }
    }
}