using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace LightningQueues
{
    public static class ReactiveExtensions
    {
        public static IObservable<T> DoAsync<T>(this IObservable<T> stream, Func<T, Task> onNext)
        {
            return from s in stream
                   from _ in Observable.FromAsync(() => onNext(s)).Catch((Exception ex) => Observable.Empty<Unit>())
                   select s;
        }

        public static IObservable<T> Using<T, TDisposable>(this IObservable<TDisposable> stream, Func<TDisposable, IObservable<T>> action) where TDisposable : IDisposable
        {
            return stream.SelectMany(x =>
            {
                return Observable.Using(() => x, action);
            });
        }
    }
}