using System;
using System.Reflection;
using FubuCore.Util;
using System.Linq;
using FubuCore.Reflection;
using System.Collections.Generic;

namespace FubuCore.CommandLine
{
    public interface IFubuCommand
    {
        void Execute(object input);
        Type InputType { get; }
    }

    public class CommandExecutor
    {
        public CommandExecutor(ICommandFactory factory)
        {
        }
    }

    public interface ICommandFactory
    {
        CommandRun BuildRun(string commandLine);
    }

    public class CommandRun
    {
        public IFubuCommand Command { get; set; }
        public object Input { get; set; }
    }

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
            assembly.GetExportedTypes().Where(x => x.Closes(typeof (FubuCommand<>))).Each(t =>
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

    // Later, think about a writer / logger
    public abstract class FubuCommand<T> : IFubuCommand
    {
        public Type InputType
        {
            get
            {
                return typeof (T);
            }
        }

        public void Execute(object input)
        {
            Execute((T)input);
        }

        public abstract void Execute(T input);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CommandDescriptionAttribute : Attribute
    {
        private readonly string _description;

        public CommandDescriptionAttribute(string description)
        {
            _description = description;
        }

        public string Description
        {
            get { return _description; }
        }

        public string Name { get; set; }
    }

    //public class LinkPackageCommand
}