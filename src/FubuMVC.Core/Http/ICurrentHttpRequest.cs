namespace FubuMVC.Core.Http
{
    public interface ICurrentHttpRequest
    {
        /// <summary>
        ///   Full url of the request, never contains a trailing /
        /// </summary>
        /// <returns></returns>
        string RawUrl();

        /// <summary>
        ///   Url relative to the application
        /// </summary>
        /// <returns></returns>
        string RelativeUrl();

        /// <summary>
        /// Gets the full url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string ToFullUrl(string url);

        string HttpMethod();
    }

    public class StandInCurrentHttpRequest : ICurrentHttpRequest
    {
        public string TheRawUrl;
        public string TheRelativeUrl;
        public string ApplicationRoot = "http://server";
        public string TheHttpMethod = "GET";

        public string RawUrl()
        {
            return TheRawUrl;
        }

        public string RelativeUrl()
        {
            return TheRelativeUrl;
        }

        public string ToFullUrl(string url)
        {
            return url;
        }

        public string HttpMethod()
        {
            return TheHttpMethod;
        }
    }
}