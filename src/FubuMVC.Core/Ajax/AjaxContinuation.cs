using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FubuCore;
using FubuCore.Util;
using FubuLocalization;

namespace FubuMVC.Core.Ajax
{
    public class AjaxContinuation
    {
        private readonly Cache<string, object> _data = new Cache<string, object>();
        private readonly IList<AjaxError> _errors = new List<AjaxError>();

        // Probably put some convenience static builder methods here for things like

        // You'll want to smuggle in your own data in all likelihood
        [IndexerName("Data")]
        public object this[string key]
        {
            get { return _data[key]; }
            set { _data[key] = value; }
        }

        // I don't care enough about Law of Demeter here
        // to do anything differently
        public IList<AjaxError> Errors
        {
            get { return _errors; }
        }

        public string NavigatePage { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public bool ShouldRefresh { get; set; }

        public static AjaxContinuation Successful()
        {
            return new AjaxContinuation
            {
                Success = true
            };
        }

        public static AjaxContinuation ForMessage(StringToken message)
        {
            return ForMessage(message.ToString());
        }

        public static AjaxContinuation ForMessage(string message)
        {
            return new AjaxContinuation
            {
                Message = message
            };
        }

        // *This* will be serialized to Json so that the resulting blob
        // of json data is easier to work with in JavaScript
        public virtual IDictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>
            {
                {"success", Success},
                {"refresh", ShouldRefresh}
            };

            Message.IfNotNull(x => dict.Add("message", x));
            NavigatePage.IfNotNull(x => dict.Add("navigatePage", x));

            _data.Each(dict.Add);

            if (_errors.Any())
            {
                dict.Add("errors", _errors.ToArray());
            }

            return dict;
        }

        public virtual bool HasData(string key)
        {
            return _data.Has(key);
        }
    }
}