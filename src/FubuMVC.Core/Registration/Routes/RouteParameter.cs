using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteParameter
    {
        // Use the first property to get the name
        // use the full blown accessor to get the value

        private readonly Accessor _accessor;
        private readonly Regex _regex;

        public RouteParameter(Accessor accessor)
        {
            _accessor = accessor;
            accessor.ForAttribute<RouteInputAttribute>(x => DefaultValue = x.DefaultValue);

            _regex = new Regex(@"{\*?" + Name + @"(?:\:.*?)?}", RegexOptions.Compiled);
        }

        public string Name { get { return _accessor.Name; } }

        public object DefaultValue { get; set; }

        public bool CanSubstitue(object inputModel)
        {
            return GetRawValue(inputModel) != null;
        }

        public string Substitute(object input, string url)
        {
            object rawValue = GetRawValue(input);
            string parameterValue = rawValue.ToString();
            return substitute(url, parameterValue);
        }

        private string substitute(string url, string parameterValue)
        {
            var encodedValue = encodeParameterValue(parameterValue);
            return _regex.Replace(url, encodedValue);
        }

        private static string encodeParameterValue(string parameterValue)
        {
            var values = parameterValue.Split('/');
            return values.Length > 0 ? values.Select(x => x.UrlEncoded()).Join("/") : values.UrlEncoded();
        }

        public string Substitute(RouteParameters parameters, string url)
        {
            return substitute(url, parameters[_accessor.Name] ?? DefaultValue.ToString());
        }

        public bool CanTemplate(object inputModel)
        {
            object rawValue = GetRawValue(inputModel);
            if (rawValue != null)
            {
                return _accessor.PropertyType.IsValueType
                           ? !rawValue.Equals(Activator.CreateInstance(_accessor.PropertyType))
                           : true;

            }
            return false;
        }

        public string ToQueryString(object input)
        {
            object rawValue = GetRawValue(input);

            return makeQueryString(rawValue);
        }

        private string makeQueryString(object rawValue)
        {
            if (rawValue == null || string.Empty.Equals(rawValue))
            {
                return Name.UrlEncoded() + "=";
            }

            return Name.UrlEncoded() + "=" + rawValue.ToString().UrlEncoded();
        }

        public string ToQueryString(RouteParameters parameters)
        {
            return makeQueryString(parameters[Name]);
        }

        private object GetRawValue(object input)
        {
            object rawValue = input == null
                                  ? DefaultValue
                                  : _accessor.GetValue(input);

            return rawValue ?? DefaultValue;
        }

        public bool IsSatisfied(RouteParameters routeParameters)
        {
            return routeParameters.Has(_accessor.Name) || DefaultValue != null;
        }
    }
}