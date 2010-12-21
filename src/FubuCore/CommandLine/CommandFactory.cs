using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuCore.CommandLine
{
    [CommandDescription("list all the available commands", Name = "help")]
    public class CommandFactory : FubuCommand<IEnumerable<Type>>, ICommandFactory
    {
        // TODO -- Make it deal well with missing commands
        private readonly Cache<string, Type> _commandTypes = new Cache<string, Type>();

        // TODO -- deal with the Help thing
        public CommandRun BuildRun(string commandLine)
        {
            var args = StringTokenizer.Tokenize(commandLine);
            return BuildRun(args);
        }

        public CommandRun BuildRun(IEnumerable<string> args)
        {
            if (!args.Any()) return HelpRun();

            var queue = new Queue<string>(args);
            var commandName = queue.Dequeue();

            var command = Build(commandName);

            var input = InputParser.BuildInput(command.InputType, queue);

            return new CommandRun{
                Command = command,
                Input = input
            };
        }

        public void RegisterCommands(Assembly assembly)
        {
            assembly
                .GetExportedTypes()
                .Where(x => x.Closes(typeof(FubuCommand<>)) && x.IsConcrete())
                .Each(t => { _commandTypes[CommandNameFor(t)] = t; });
        }

        public static string CommandNameFor(Type type)
        {
            var name = type.Name.TrimEnd("Command".ToCharArray()).ToLower();
            type.ForAttribute<CommandDescriptionAttribute>(att => name = att.Name ?? name);

            return name;
        }

        public static string DescriptionFor(Type type)
        {
            var description = type.FullName;
            type.ForAttribute<CommandDescriptionAttribute>(att => description = att.Description);

            return description;
        }

        public IFubuCommand Build(string commandName)
        {
            return (IFubuCommand) Activator.CreateInstance(_commandTypes[commandName.ToLower()]);
        }

        public override void Execute(IEnumerable<Type> types)
        {
            var report = new TwoColumnReport("Available commands:");
            types.OrderBy(CommandNameFor).Each(type =>
            {
                report.Add(CommandNameFor(type), DescriptionFor(type));
            });

            report.Write();
        }

        public virtual CommandRun HelpRun()
        {
            return new CommandRun(){
                Command = this,
                Input = _commandTypes.GetAll()
            };
        }
    }
}