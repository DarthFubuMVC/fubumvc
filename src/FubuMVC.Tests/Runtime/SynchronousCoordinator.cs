using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Tests.Runtime
{
    public class SynchronousCoordinator : IAsyncCoordinator
    {
        public void Push(Task task)
        {
            task.Wait();
        }
    }
}