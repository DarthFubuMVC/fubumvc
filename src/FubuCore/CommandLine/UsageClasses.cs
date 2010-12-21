using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Reflection;

namespace FubuCore.CommandLine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UsageAttribute : Attribute
    {
        private readonly string _description;
        private readonly string _name;

        public UsageAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _description; }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredUsageAttribute : Attribute
    {
        private readonly string[] _usages;

        public RequiredUsageAttribute(params string[] usages)
        {
            _usages = usages;
        }

        public string[] Usages
        {
            get { return _usages; }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ValidUsageAttribute : Attribute
    {
        private readonly string[] _usages;

        public ValidUsageAttribute(params string[] usages)
        {
            _usages = usages;
        }

        public string[] Usages
        {
            get { return _usages; }
        }
    }


    // To match the usage, gotta use the fattest one first
    // Test this in the morning
    public class CommandUsage
    {
        public string UsageKey { get; set; }
        public string CommandName { get; set; }
        public string Description { get; set; }
        public IEnumerable<ITokenHandler> Mandatories { get; set; }
        public IEnumerable<ITokenHandler> Flags { get; set; }

        public string Usage
        {
            get
            {
                return "fubu {0} {1}".ToFormat(CommandName,
                                               Mandatories.Union(Flags).Select(x => x.ToUsageDescription()).Join(" "));
            }
        }

        public bool Matches(IEnumerable<ITokenHandler> actuals)
        {
            return Mandatories.All(x => actuals.Contains(x));
        }
    }

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
            Console.WriteLine("Usages for '{0}'", _commandName);

            var usageReport = new TwoColumnReport("Usages");
            _usages.OrderBy(x => x.Mandatories.Count()).Each(u =>
            {
                usageReport.Add(u.Description, u.Usage);
            });
            usageReport.Write();

            var argumentReport = new TwoColumnReport("Arguments");
            Arguments.Each(x => argumentReport.Add(x.PropertyName.ToLower(), x.Description));
            argumentReport.Write();

            var flagReport = new TwoColumnReport("Flags");
            Flags.Each(x => flagReport.Add(x.Description, x.ToUsageDescription()));
            flagReport.Write();
        }
    }
}