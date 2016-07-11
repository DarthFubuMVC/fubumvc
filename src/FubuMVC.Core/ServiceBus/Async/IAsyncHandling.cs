using System;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Async
{
    [Obsolete("This needs to go away. Will be completely unnecessary")]
    public interface IAsyncHandling
    {
        void Push(Task task);
        void Push<T>(Task<T> task);

        void WaitForAll(); // can throw aggregate exception
    }
}