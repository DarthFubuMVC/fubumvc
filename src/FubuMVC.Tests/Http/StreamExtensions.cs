using System.IO;

namespace FubuMVC.Tests.Http
{
    public static class StreamExtensions
    {
        public static Stream AsStream(this string text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(text);
            writer.Flush();

            stream.Position = 0;
            return stream;
        }
    }
}