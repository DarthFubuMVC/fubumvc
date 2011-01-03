using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.CommandLine
{
    public class UsageGraph
    {
        private readonly string _commandName;
        private readonly Type _commandType;
        private readonly IList<CommandUsage> _usages = new List<CommandUsage>();
        private string _description;
        private readonly Type _inputType;
        private readonly List<ITokenHandler> _tokens;

        public UsageGraph(Type commandType)
        {
            _commandType = commandType;
            _inputType = commandType.FindInterfaceThatCloses(typeof (IFubuCommand<>)).GetGenericArguments().First();

            _commandName = CommandFactory.CommandNameFor(commandType);
            _commandType.ForAttribute<CommandDescriptionAttribute>(att => { _description = att.Description; });

            if (_description == null) _description = _commandType.Name;

            _tokens = InputParser.GetHandlers(_inputType);

            _commandType.ForAttribute<UsageAttribute>(att =>
            {
                _usages.Add(buildUsage(att));
            });

            if (!_usages.Any())
            {
                var usage = new CommandUsage(){
                    CommandName = _commandName,
                    UsageKey = "default",
                    Description = _description,
                    Mandatories = _tokens.Where(x => x is Argument),
                    Flags = _tokens.Where(x => !(x is Argument))
                };

                _usages.Add(usage);
            }

        }

        private CommandUsage buildUsage(UsageAttribute att)
        {
            return new CommandUsage(){
                CommandName = _commandName,
                UsageKey = att.Name,
                Description = att.Description,
                Mandatories = _tokens.Where(x => x.RequiredForUsage(att.Name)),
                Flags = _tokens.Where(x => x.OptionalForUsage(att.Name))
            };
        }

        public CommandUsage FindUsage(string key)
        {
            return _usages.FirstOrDefault(x => x.UsageKey == key);
        }

        public string CommandName
        {
            get { return _commandName; }
        }

        public IEnumerable<Argument> Arguments
        {
            get
            {
                return _tokens.OfType<Argument>();
            }
        }
        public IEnumerable<ITokenHandler> Flags
        {
            get
            {
                return _tokens.Where(x => !(x is Argument));
            }
        }

        public IEnumerable<CommandUsage> Usages
        {
            get { return _usages; }
        }

        public string Description
        {
            get { return _description; }
        }

        public void WriteUsages()
        {
            if (!_usages.Any())
            {
                Console.WriteLine("No documentation for this command");
                return;
            }

            Console.WriteLine(" Usages for '{0}'", _commandName);
            Console.WriteLine(" " + _description);

            if (_usages.Count == 1)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" " + _usages.Single().Usage);
                Console.ResetColor();
            }
            else
            {
                writeMultipleUsages();
            }

            writeArguments();


            if (!Flags.Any()) return;

            writeFlags();
        }

        private void writeMultipleUsages()
        {
            var usageReport = new TwoColumnReport("Usages"){
                SecondColumnColor = ConsoleColor.Cyan
            };

            _usages.OrderBy(x => x.Mandatories.Count()).Each(u =>
            {
                usageReport.Add(u.Description, u.Usage);
            });

            usageReport.Write();
        }

        private void writeArguments()
        {
            var argumentReport = new TwoColumnReport("Arguments");
            Arguments.Each(x => argumentReport.Add(x.PropertyName.ToLower(), x.Description));
            argumentReport.Write();
        }

        private void writeFlags()
        {
            var flagReport = new TwoColumnReport("Flags");
            Flags.Each(x => flagReport.Add(x.ToUsageDescription(), x.Description));
            flagReport.Write();
        }
    }
}