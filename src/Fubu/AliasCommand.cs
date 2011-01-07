using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.CommandLine;

namespace Fubu
{
    public class AliasInput
    {
        [RequiredUsage("create", "remove")]
        [Description("The name of the alias")]
        public string Name { get; set; }

        [RequiredUsage("create")]
        [Description("The path to the actual folder")]
        public string Folder { get; set; }

        [ValidUsage("remove")]
        [Description("Removes the alias")]
        public bool RemoveFlag { get; set; }
    }


    [Usage("list", "List all the aliases for this solution folder")]
    [Usage("create", "Creates a new alias for a folder")]
    [Usage("remove", "Removes an alias")]
    [CommandDescription("Manage folder aliases")]
    public class AliasCommand : FubuCommand<AliasInput>
    {
        public static string AliasFolder(string folder)
        {
            var alias = new FileSystem()
                .LoadFromFile<AliasRegistry>(AliasRegistry.ALIAS_FILE)
                .AliasFor(folder);

            return alias == null ? folder : alias.Folder;
        }


        public override void Execute(AliasInput input)
        {
            Execute(input, new FileSystem());
        }

        public void Execute(AliasInput input, IFileSystem system)
        {
            var registry = system.LoadFromFile<AliasRegistry>(AliasRegistry.ALIAS_FILE);
            if (input.Name.IsEmpty())
            {
                writeAliases(registry);
                return;
            }

            if (input.RemoveFlag)
            {
                registry.RemoveAlias(input.Name);
                Console.WriteLine("Alias {0} removed", input.Name);
            }
            else
            {
                registry.CreateAlias(input.Name, input.Folder);
                Console.WriteLine("Alias {0} created for folder {1}", input.Name, input.Folder);
            }

            persist(system, registry);
        }

        private void writeAliases(AliasRegistry registry)
        {
            if (!registry.Aliases.Any())
            {
                Console.WriteLine(" No aliases are registered");
                return;
            }

            var maximumLength = registry.Aliases.Select(x => x.Name.Length).Max();
            var format = "  {0," + maximumLength + "} -> {1}";

            Console.WriteLine();
            Console.WriteLine(
                "----------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine(" Aliases:");
            Console.WriteLine(
                "----------------------------------------------------------------------------------------------------------------------");

            registry.Aliases.OrderBy(x => x.Name).Each(x => { Console.WriteLine(format, x.Name, x.Folder); });
            Console.WriteLine(
                "----------------------------------------------------------------------------------------------------------------------");
        }

        private void persist(IFileSystem system, AliasRegistry registry)
        {
            system.WriteObjectToFile(AliasRegistry.ALIAS_FILE, registry);
        }
    }

    [XmlType("aliases")]
    public class AliasRegistry
    {
        public static readonly string ALIAS_FILE = ".fubu-alias";

        private readonly IList<AliasToken> _aliases = new List<AliasToken>();

        public AliasToken[] Aliases
        {
            get { return _aliases.ToArray(); }
            set
            {
                _aliases.Clear();
                _aliases.AddRange(value);
            }
        }

        public void CreateAlias(string alias, string folder)
        {
            var token = AliasFor(alias);
            if (token == null)
            {
                token = new AliasToken{
                    Folder = folder,
                    Name = alias
                };

                _aliases.Add(token);
            }
            else
            {
                token.Folder = folder;
            }
        }

        public AliasToken AliasFor(string alias)
        {
            return _aliases.SingleOrDefault(x => x.Name == alias);
        }

        public void RemoveAlias(string alias)
        {
            _aliases.RemoveAll(x => x.Name == alias);
        }
    }

    [XmlType("alias")]
    public class AliasToken
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("folder")]
        public string Folder { get; set; }
    }
}