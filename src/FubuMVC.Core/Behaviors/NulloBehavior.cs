using System.Threading.Tasks;

namespace FubuMVC.Core.Behaviors
{
    public class NulloBehavior : IActionBehavior
    {
        public Task Invoke()
        {
            return Task.CompletedTask;
        }

        public Task InvokePartial()
        {
            return Task.CompletedTask;
        }
    }
}