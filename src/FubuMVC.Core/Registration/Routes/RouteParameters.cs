using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteParameters
    {
        private readonly Cache<string, string> _parameters = new Cache<string, string>(key => null);

        public string this[string name]
        {
            get { return _parameters[name]; }
            set { _parameters[name] = value; }
        }

        public IEnumerable<string> AllNames
        {
            get { return _parameters.GetAllKeys(); }
        }

        public bool Has(string name)
        {
            return _parameters.Has(name);
        }

        public override string ToString()
        {
            return "parameters:  " + _parameters.GetAllKeys().Join(", ");
        }
    }

    public class RouteParameters<T> : RouteParameters
    {
        public string this[Expression<Func<T, object>> expression]
        {
            get { return this[expression.ToAccessor().Name]; }
            set { this[expression.ToAccessor().Name] = value; }
        }
    }
}