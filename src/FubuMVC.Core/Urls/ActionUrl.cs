using System;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Routes;

namespace FubuMVC.Core.Urls
{
    public class ActionUrl
    {
        private readonly IRouteDefinition _route;

        public ActionUrl(IRouteDefinition route, ActionCall call)
        {
            _route = route;
            HandlerType = call.HandlerType;
            Method = call.Method;
        }

        public Type HandlerType { get; set; }
        public MethodInfo Method { get; set; }

        public string GetUrl(object input)
        {
            return _route.CreateUrl(input);
        }

        public bool Equals(ActionUrl other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._route, _route) && Equals(other.HandlerType, HandlerType) &&
                   Equals(other.Method, Method);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ActionUrl)) return false;
            return Equals((ActionUrl) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (_route != null ? _route.GetHashCode() : 0);
                result = (result*397) ^ (HandlerType != null ? HandlerType.GetHashCode() : 0);
                result = (result*397) ^ (Method != null ? Method.GetHashCode() : 0);
                return result;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} --> {1}.{2}()", _route, HandlerType, Method);
        }
    }
}