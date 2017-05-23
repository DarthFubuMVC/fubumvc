using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LightningQueues.Tests
{
    public static class ReactiveExtensions
    {
        public static async Task<T> FirstAsyncWithTimeout<T>(this IObservable<T> stream, TimeSpan timeSpan)
        {
            var completionSource = new TaskCompletionSource<T>();
            using (stream.Subscribe(x => completionSource.SetResult(x)))
            using (Observable.Interval(timeSpan).Subscribe(x => completionSource.SetException(new TimeoutException())))
            {
                return await completionSource.Task;
            }
        }

        public static Task<T> FirstAsyncWithTimeout<T>(this IObservable<T> stream)
        {
            return stream.FirstAsyncWithTimeout(TimeSpan.FromSeconds(1));
        }

        public static IObservable<int> RunningCount<T>(this IObservable<T> stream)
        {
            return stream.Scan(0, (acc, current) => acc + 1);
        }

        public static IObservable<int> RunningSum(this IObservable<int> stream)
        {
            return stream.Scan(0, (acc, current) => acc + current);
        }

        public static IObservable<T> ThrowTimes<T>(this IObservable<T> stream, int retries)
        {
            var count = 0;
            return Observable.Create<T>(sub =>
            {
                count++;
                if(count > retries)
                    sub.OnCompleted();
                else
                    sub.OnError(new Exception());
                return Disposable.Empty;
            }).Concat(stream);
        }
    }
}