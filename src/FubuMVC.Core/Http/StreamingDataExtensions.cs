using System.IO;

namespace FubuMVC.Core.Http
{
    public static class StreamingDataExtensions
    {
        /// <summary>
        /// Helper function to read the response body as a string with the default content encoding
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string InputText(this IStreamingData data)
        {
            var reader = new StreamReader(data.Input);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Checks whether or not there is any data in the request body
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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