using System;
using System.Configuration;
using FubuCore.Binding;
using System.Linq;

namespace FubuCore.Configuration
{
    public class AppSettingsRequestData : IRequestData
    {
        public object Value(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public bool Value(string key, Action<object> callback)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                callback(Value(key));
                return true;
            }

            return false;
        }
    }
}