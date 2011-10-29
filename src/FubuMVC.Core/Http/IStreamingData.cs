using System.IO;

namespace FubuMVC.Core.Http
{
    public interface IStreamingData
    {
        Stream Input { get; }
    }

}