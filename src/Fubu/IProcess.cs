namespace Fubu
{
    public interface IProcess
    {
        bool Start();
        void WaitForExit();
        int ExitCode { get; }
    }
}