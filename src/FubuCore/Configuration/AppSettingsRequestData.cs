using System;
using System.Configuration;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Core.Configuration
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