using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Tests.Runtime
{
    public class SynchronousCoordinator : IAsyncCoordinator
    {
        public void Push(params Task[] tasks)
        {
            tasks.Each(x => x.Wait());
        }
    }
}