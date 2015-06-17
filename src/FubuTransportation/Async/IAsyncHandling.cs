using System.Threading.Tasks;

namespace FubuTransportation.Async
{
    public interface IAsyncHandling
    {
        void Push(Task task);
        void Push<T>(Task<T> task);

        void WaitForAll(); // can throw aggregate exception
    }
}