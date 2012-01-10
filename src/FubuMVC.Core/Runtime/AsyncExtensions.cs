using System.Linq;
using System.Threading.Tasks;

namespace FubuMVC.Core.Runtime
{
    public static class AsyncExtensions
    {
         public static void FinishProcessingTask(this Task task, IExceptionHandlingObserver observer)
         {
             if (task.IsFaulted)
             {
                 var aggregateException = task.Exception.Flatten();
                 var allHandled = aggregateException.InnerExceptions.All(observer.WasObserved);
                 if (!allHandled)
                 {
                     task.Wait();
                 }
             }
         }
    }
}