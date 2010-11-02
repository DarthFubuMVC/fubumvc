namespace FubuCore.CommandLine
{
    public class CommandExecutor
    {
        private readonly ICommandFactory _factory;

        public CommandExecutor(ICommandFactory factory)
        {
            _factory = factory;
        }

        public void Execute(string commandLine)
        {
            var run = _factory.BuildRun(commandLine);
            run.Execute();
        }
    }
}