using System.IO;

namespace FubuMVC.Core.Http
{
    /// <summary>
    /// Provides access to the request body as a Stream
    /// </summary>
    public interface IStreamingData
    {
        Stream Input { get; }
    }

}