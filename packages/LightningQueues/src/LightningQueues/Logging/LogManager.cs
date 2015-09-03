using System;

namespace LightningQueues.Logging
{
    public static class LogManager
    {
        public static ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public static ILogger GetLogger(Type type)
        {
            return new NLogLogger(NLog.LogManager.GetLogger(type.FullName));
        }
    }
}