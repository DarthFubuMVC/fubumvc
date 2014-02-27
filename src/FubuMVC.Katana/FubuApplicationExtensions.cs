using FubuMVC.Core;

namespace FubuMVC.Katana
{
    public static class FubuApplicationExtensions
    {
        /// <summary>
        /// Creates an embedded web server for this FubuApplication running at the designated physical path and port
        /// </summary>
        /// <param name="application"></param>
        /// <param name="physicalPath">The physical path of the web server path.  This only needs to be set if the location for application content like scripts or views is at a different place than the current AppDomain base directory</param>
        /// <param name="port">The port to run the web server at.  The web server will try other port numbers starting at this point if it is unable to bind to this specific port.  If the port is zero or less, EmbeddedFubuMvcServer will use the first open port starting from 5500</param>
        /// <returns></returns>
        public static EmbeddedFubuMvcServer RunEmbedded(this FubuApplication application, string physicalPath = null,
            int port = 5500)
        {
            return new EmbeddedFubuMvcServer(application.Bootstrap(), physicalPath, port);
        }

        /// <summary>
        /// Creates an embedded web server for this FubuApplication running at the designated physical path and port
        /// Automatically selects a free port starting at 5500
        /// </summary>
        /// <param name="application"></param>
        /// <param name="physicalPath">The physical path of the web server path.  This only needs to be set if the location for application content like scripts or views is at a different place than the current AppDomain base directory</param>
        /// <returns></returns>
        public static EmbeddedFubuMvcServer RunEmbeddedWithAutoPort(this FubuApplication application, string physicalPath = null)
        {
            return new EmbeddedFubuMvcServer(application.Bootstrap(), physicalPath, 0);
        }
    }
}