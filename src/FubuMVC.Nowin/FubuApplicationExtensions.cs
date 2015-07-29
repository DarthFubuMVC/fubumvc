using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;

namespace FubuMVC.Nowin
{
    public static class FubuApplicationExtensions
    {
        /// <summary>
        /// Creates an embedded web server for this FubuApplication running at the designated physical path and port
        /// </summary>
        /// <param name="application"></param>
        /// <param name="port">The port to run the web server at.  The web server will try other port numbers starting at this point if it is unable to bind to this specific port.  If the port is zero or less, EmbeddedFubuMvcServer will use the first open port starting from 5500</param>
        /// <returns></returns>
        public static EmbeddedFubuMvcServer RunEmbedded(this FubuApplication application, int port = 5500)
        {
            return new EmbeddedFubuMvcServer(application.Bootstrap(), new NowinHost(), port);
        }

        /// <summary>
        /// Creates an embedded web server for this FubuApplication running at the designated physical path and port
        /// Automatically selects a free port starting at 5500
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public static EmbeddedFubuMvcServer RunEmbeddedWithAutoPort(this FubuApplication application)
        {
            return new EmbeddedFubuMvcServer(application.Bootstrap(), new NowinHost(), 0);
        }
    }
}
