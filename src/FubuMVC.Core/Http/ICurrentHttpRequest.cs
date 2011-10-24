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
        ///   Base root of the application
        /// </summary>
        /// <returns></returns>
        string ApplicationRoot();

        string HttpMethod();
    }
}