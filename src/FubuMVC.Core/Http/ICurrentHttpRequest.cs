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
}