using System.Collections.Generic;

namespace FubuMVC.Core.Http.Owin
{
    public class OwinHeaderSettings
    {
        private readonly IList<string> _singularHeaders = new List<string>();

        public OwinHeaderSettings()
        {
            DoNotAllowMultipleValues(HttpRequestHeaders.ContentLength);
        }

        public void DoNotAllowMultipleValues(string key)
        {
            _singularHeaders.Add(key);
        }

        public bool AllowMultiple(string key)
        {
            return !_singularHeaders.Contains(key);
        }
    }
}
