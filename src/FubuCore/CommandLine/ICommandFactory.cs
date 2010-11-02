namespace FubuCore.CommandLine
{
    public interface ICommandFactory
    {
        CommandRun BuildRun(string commandLine);
    }
}