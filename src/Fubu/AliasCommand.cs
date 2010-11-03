using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.CommandLine;
using System.Linq;

namespace Fubu
{
    public class AliasInput
    {
        public string Name { get; set; }
        public string Folder { get; set; }
        public bool RemoveFlag { get; set; }
    }

    public class AliasCommand : FubuCommand<AliasInput>
    {
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
            }
            else
            {
                registry.CreateAlias(input.Name, input.Folder);
            }

            persist(system, registry);
        }

        private void writeAliases(AliasRegistry registry)
        {
            var maximumLength = registry.Aliases.Select(x => x.Name.Length).Max();
            var format = "  {0," + maximumLength + "} -> {1}";

            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Aliases:");
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");

            registry.Aliases.OrderBy(x => x.Name).Each(x =>
            {
                Console.WriteLine(format, x.Name, x.Folder);
            });
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
        }

        private void persist(IFileSystem system, AliasRegistry registry)
        {
            system.PersistToFile(registry, AliasRegistry.ALIAS_FILE);
        }
    }

    [XmlType("aliases")]
    public class AliasRegistry
    {
        public static readonly string ALIAS_FILE = ".fubu-alias";

        private readonly IList<AliasToken> _aliases = new List<AliasToken>();

        public AliasToken[] Aliases
        {
            get
            {
                return _aliases.ToArray();
            }
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
                token = new AliasToken(){
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