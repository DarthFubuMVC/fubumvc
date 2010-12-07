using FubuCore.CommandLine;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuCore.Testing.CommandLine
{
    // TODO -- switch to BDD style later
    [TestFixture]
    public class CommandExecutorTester
    {
        private ICommandFactory factory;
        private IFubuCommand command;
        private CommandExecutor theExecutor;
        private object theInput;
        private string commandLine;

        [SetUp]
        public void SetUp()
        {
            factory = MockRepository.GenerateMock<ICommandFactory>();
            command = MockRepository.GenerateMock<IFubuCommand>();
            theInput = new object();
            commandLine = "some stuff here";

            theExecutor = new CommandExecutor(factory);
        }

        [Test]
        public void run_command_happy_path_executes_the_command_with_the_input()
        {
            factory.Stub(x => x.BuildRun(commandLine)).Return(new CommandRun(){
                Command = command,
                Input = theInput
            });

            theExecutor.Execute(commandLine);

            command.AssertWasCalled(x => x.Execute(theInput));
        }
    }
}