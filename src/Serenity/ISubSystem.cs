using System.Threading.Tasks;
using FubuCore;

namespace Serenity
{
    public interface ISubSystem
    {
        Task Start();
        Task Stop();
    }
}