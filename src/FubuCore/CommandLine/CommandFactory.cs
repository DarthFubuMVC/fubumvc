using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuCore.Util;

namespace FubuCore.CommandLine
{
    public class HelpInput
    {
        [IgnoreOnCommandLine]
        public IEnumerable<Type> CommandTypes { get; set; }

        [RequiredUsage("usage")]
        [Description("A command name")]
        public string Name { get; set; }

        [IgnoreOnCommandLine]
        public bool InvalidCommandName { get; set; }

        [IgnoreOnCommandLine]
        public UsageGraph Usage { get; set; }
    }
    
    // TODO -- test this nonsense
    [Usage("list", "List all the available commands")]
    [Usage("usage", "Show all the valid usages for a command")]
    [CommandDescription("list all the available commands", Name = "help")]
    public class HelpCommand : FubuCommand<HelpInput>
    {
        // TODO -- have it write out its own usage
        // TODO -- look for command line stuff
        public override void Execute(HelpInput input)
        {
            if (input.Usage != null)
            {
                input.Usage.WriteUsages();
                return;
            }

            if (input.InvalidCommandName)
            {
                writeInvalidCommand(input.Name);
            }

            listAllCommands(input);
        }

        private void listAllCommands(HelpInput input)
        {
            var report = new TwoColumnReport("Available commands:");
            input.CommandTypes.OrderBy(CommandFactory.CommandNameFor).Each(type =>
            {
                report.Add(CommandFactory.CommandNameFor(type), CommandFactory.DescriptionFor(type));
            });

            report.Write();
        }

        private void writeInvalidCommand(string commandName)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("fubu:  '{0}' is not a command.  See available commands.", commandName);
            Console.WriteLine();
            Console.ResetColor();
        }
    }



    public class CommandFactory : ICommandFactory
    {
        private static readonly string[] _helpCommands = new string[]{"help", "?"}; 
        private readonly Cache<string, Type> _commandTypes = new Cache<string, Type>();

        // TODO -- deal with the Help thing
        public CommandRun BuildRun(string commandLine)
        {
            var args = StringTokenizer.Tokenize(commandLine);
            return BuildRun(args);
        }

        public CommandRun BuildRun(IEnumerable<string> args)
        {
            if (!args.Any()) return HelpRun(new Queue<string>());

            var queue = new Queue<string>(args);
            var commandName = queue.Dequeue().ToLowerInvariant();

            // TEMPORARY
            if (_helpCommands.Contains(commandName))
            {
                return HelpRun(queue);
            }

            if (_commandTypes.Has(commandName))
            {
                return buildRun(queue, commandName);
            }

            return InvalidCommandRun(commandName);
        }

        public IEnumerable<Type> AllCommandTypes()
        {
            return _commandTypes.GetAll();
        }

        public CommandRun InvalidCommandRun(string commandName)
        {
            return new CommandRun()
            {
                Command = new HelpCommand(),
                Input = new HelpInput(){
                    Name = commandName,
                    CommandTypes = _commandTypes.GetAll(),
                    InvalidCommandName = true
                }
            };
        }

        private CommandRun buildRun(Queue<string> queue, string commandName)
        {
            var command = Build(commandName);

            // this is where we'll call into UsageGraph?
            try
            {
                var usageGraph = new UsageGraph(_commandTypes[commandName]);
                var input = usageGraph.BuildInput(queue);

                return new CommandRun
                       {
                           Command = command,
                           Input = input
                       };
            }
            catch (InvalidUsageException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid usage");

                if (e.Message.IsNotEmpty())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(e.Message);
                }

                Console.ResetColor();
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error parsing input");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(e);
                Console.ResetColor();
                Console.WriteLine();
            }

            return HelpRun(commandName);
        }



        public void RegisterCommands(Assembly assembly)
        {
            assembly
                .GetExportedTypes()
                .Where(x => x.Closes(typeof(FubuCommand<>)) && x.IsConcrete())
                .Each(t => { _commandTypes[CommandNameFor(t)] = t; });
        }



        public IFubuCommand Build(string commandName)
        {
            return (IFubuCommand) Activator.CreateInstance(_commandTypes[commandName.ToLower()]);
        }



        public CommandRun HelpRun(string commandName)
        {
            return HelpRun(new Queue<string>(new []{commandName}));
        }

        public virtual CommandRun HelpRun(Queue<string> queue)
        {
            var input = (HelpInput) (new UsageGraph(typeof (HelpCommand)).BuildInput(queue));
            input.CommandTypes = _commandTypes.GetAll();


            if (input.Name.IsNotEmpty())
            {
                input.InvalidCommandName = true;
                input.Name = input.Name.ToLowerInvariant();
                _commandTypes.WithValue(input.Name, type =>
                {
                    input.InvalidCommandName = false;
                    input.Usage = new UsageGraph(type);
                });
            }

            return new CommandRun(){
                Command = new HelpCommand(),
                Input = input
            };
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
    }
}