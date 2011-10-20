using System.IO;

namespace FubuMVC.Core.Http
{
    public static class StreamingDataExtensions
    {
        public static string InputText(this IStreamingData data)
        {
            var reader = new StreamReader(data.Input);
            return reader.ReadToEnd();
        }

        public static bool HasBodyData(this IStreamingData data)
        {
            return data.Input != null && data.Input.CanRead && data.Input.Length > 0;
        }

        public static bool CouldBeJson(this IStreamingData data)
        {
            if (!data.HasBodyData()) return false;

            var reader = new StreamReader(data.Input);
            var firstCharacter = reader.Read();
            data.Input.Position = 0;

            return firstCharacter == '{';
        }
    }
}