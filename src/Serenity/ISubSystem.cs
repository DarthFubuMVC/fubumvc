using System.Threading.Tasks;
using Bottles.Services.Messaging;
using FubuCore;

namespace Serenity
{
    public interface ISubSystem
    {
        // TODO -- this should be in Storyteller itself(?)
        Task Start();
        Task Stop();
    }
}