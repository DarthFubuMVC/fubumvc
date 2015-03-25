using System;
using System.Threading;

namespace Serenity
{
    public static class Retry
    {
        public static T Twice<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception)
            {
                Thread.Sleep(100);
                return func();
            }
        }

        public static void Twice(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                Thread.Sleep(100);
                action();
            }
        }

        public static T FiveTimes<T>(Func<T> func)
        {
            var count = 0;
            while (count < 5)
            {
                try
                {
                    return func();
                }
                catch (Exception)
                {
                    if (count >= 4)
                    {
                        throw;
                    }

                    count++;
                    Thread.Sleep(100);
                }
            }

            throw new InvalidOperationException("Just did not work");
        }

        public static void FiveTimes(Action action)
        {
            var count = 0;
            while (count < 5)
            {
                try
                {
                    action();
                }
                catch (Exception)
                {
                    if (count >= 4)
                    {
                        throw;
                    }

                    count++;
                    Thread.Sleep(100);
                }
            }

            throw new InvalidOperationException("Just did not work");
        }
    }
}