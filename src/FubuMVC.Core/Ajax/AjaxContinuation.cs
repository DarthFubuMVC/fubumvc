using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Ajax
{
    /// <summary>
    /// Used as the return value from Ajax endpoints to standardize the communication and interaction of the server
    /// and client.  Several built in conventions and external Bottles work with AjaxContinuation
    /// </summary>
    public class AjaxContinuation
    {
        private readonly Cache<string, object> _data = new Cache<string, object>();
        private readonly IList<AjaxError> _errors = new List<AjaxError>();


        /// <summary>
        /// Apply key/value pairs
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [IndexerName("Data")]
        public object this[string key]
        {
            get { return _data[key]; }
            set { _data[key] = value; }
        }

        /// <summary>
        /// List of AjaxError's to communicate validation errors to the client
        /// </summary>
        public IList<AjaxError> Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// If expressed, tells the client page to navigate the browser to the designated url
        /// </summary>
        public string NavigatePage { get; set; }

        public bool Success { get; set; }
        
        public string Message { get; set; }

        /// <summary>
        /// Should the entire page or at least the part of the UI that originated the Ajax call be refreshed?
        /// </summary>
        public bool ShouldRefresh { get; set; }

        /// <summary>
        /// Convenience method to get an empty AjaxContinuation{Success = true}
        /// </summary>
        /// <returns></returns>
        public static AjaxContinuation Successful()
        {
            return new AjaxContinuation
            {
                Success = true
            };
        }

        /// <summary>
        /// Convenience method to build an AjaxContinuation for a failure message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static AjaxContinuation ForMessage(object message)
        {
            return ForMessage(message.ToString());
        }

        /// <summary>
        /// Convenience method to build an AjaxContinuation for a failure message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static AjaxContinuation ForMessage(string message)
        {
            return new AjaxContinuation
            {
                Message = message
            };
        }

        /// <summary>
        /// Builds a dictionary that FubuMVC will serialize to Json
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Does this AjaxContinuation have a value for the key?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool HasData(string key)
        {
            return _data.Has(key);
        }
    }
}