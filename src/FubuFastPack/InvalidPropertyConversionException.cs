using System;

namespace FubuFastPack
{
    public class InvalidPropertyConversionException : Exception
    {
        private InvalidPropertyConversionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static Exception For(string format, Exception exception)
        {
            return new InvalidPropertyConversionException(format, exception);
        }
    }
}