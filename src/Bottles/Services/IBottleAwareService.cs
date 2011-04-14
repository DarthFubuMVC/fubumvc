namespace Bottles.Services
{
    /// <summary>
    /// The clients entry point into the process.
    /// </summary>
    public interface IBottleAwareService : IBootstrapper
    {
        void Stop();
    }
}