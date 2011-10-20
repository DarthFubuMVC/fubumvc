using System.IO;

namespace FubuMVC.Core.Http
{
    public interface IStreamingData
    {
        Stream Input { get; }
        Stream Output { get;}
        string OutputContentType { get; set; }
    }

}