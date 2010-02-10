using System;
using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Models
{
    public class RawValue
    {
        public PropertyInfo Property;
        public object Value;

        public IBindingContext Context;
    }

    public delegate object ValueConverter(RawValue value);
}