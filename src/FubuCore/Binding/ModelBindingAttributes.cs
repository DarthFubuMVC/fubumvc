using System;

namespace FubuMVC.Core.Models
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExpandEnvironmentVariablesAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MapFromWebPathAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MapWebToPhysicalPathAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConnectionStringAttribute : Attribute { }

}