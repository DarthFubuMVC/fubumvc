using System;
using FubuCore;
using FubuCore.Reflection;

namespace FubuMVC.Core.Registration.Routes
{
    public class RouteInput
    {
        // Use the first property to get the name
        // use the full blown accessor to get the value

        private readonly Accessor _accessor;

        public RouteInput(Accessor accessor)
        {
            _accessor = accessor;
            accessor.ForAttribute<RouteInputAttribute>(x => DefaultValue = x.DefaultValue);
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
            return url.Replace("{" + Name + "}", rawValue.ToString().UrlEncoded());
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

            if (rawValue == null || string.Empty.Equals(rawValue))
            {
                return Name.UrlEncoded() + "=";
            }

            return Name.UrlEncoded() + "=" + rawValue.ToString().UrlEncoded();
        }

        private object GetRawValue(object input)
        {
            object rawValue = input == null
                                  ? DefaultValue
                                  : _accessor.GetValue(input);

            return rawValue ?? DefaultValue;
        }
    }
}