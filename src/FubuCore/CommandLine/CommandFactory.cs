using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuCore.CommandLine
{
    public class CommandFactory : ICommandFactory
    { 
        // TODO -- Make it deal well with missing commands
        private readonly Cache<string, Type> _commandTypes = new Cache<string, Type>();

        // TODO -- deal with the Help thing
        public CommandRun BuildRun(string commandLine)
        {
            var tokens = StringTokenizer.Tokenize(commandLine);
            var queue = new Queue<string>(tokens);
            var commandName = queue.Dequeue();

            var command = Build(commandName);
            
            var parser = new InputParser();
            var input = parser.BuildInput(command.InputType, queue);

            return new CommandRun(){
                Command = command,
                Input = input
            };
        }

        public void RegisterCommands(Assembly assembly)
        {
            assembly
                .GetExportedTypes()
                .Where(x => x.Closes(typeof (FubuCommand<>)))
                .Each(t =>
            {
                _commandTypes[CommandNameFor(t)] = t;
            });
        }

        public static string CommandNameFor(Type type)
        {
            var name = type.Name.TrimEnd("Command".ToCharArray()).ToLower();
            type.ForAttribute<CommandDescriptionAttribute>(att => name = att.Name ?? name);

            return name;
        }

        public IFubuCommand Build(string commandName)
        {
            return (IFubuCommand) Activator.CreateInstance(_commandTypes[commandName.ToLower()]);
        }
    }
}