using System;

namespace FubuCore
{
    public static class BasicExtensions
    {
        public static TOut IfNotNull<T, TOut>(this T target, Func<T, TOut> valueFunc)
            where T : class
        {
            return target == null ? default(TOut) : valueFunc(target);
        }

        public static string IfNotNull<T>(this T target, Func<T, string> valueFunc)
            where T : class
        {
            return target == null ? null : valueFunc(target);
        }

        public static T IfNotNull<T>(this object target, Func<T> valueFunc)
            where T : class
        {
            return target == null ? null : valueFunc();
        }

        public static void SafeDispose(this IDisposable disposable)
        {
            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
                // That's right, swallow that exception
            }
        }
    }
}