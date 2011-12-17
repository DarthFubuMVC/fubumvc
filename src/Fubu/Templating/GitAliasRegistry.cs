using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Fubu.Templating
{
    [XmlType("aliases")]
    public class GitAliasRegistry
    {
        public static readonly string ALIAS_FILE = ".fubunew-alias";

        private readonly IList<GitAliasToken> _aliases = new List<GitAliasToken>();

        public GitAliasRegistry()
        {
            setupDefaults();
        }

        public GitAliasToken[] Aliases
        {
            get { return _aliases.ToArray(); }
            set
            {
                setupDefaults();
                value.Each(token => CreateAlias(token.Name, token.Url));
            }
        }

        private void setupDefaults()
        {
            _aliases.Clear();
            CreateAlias("fububottle", "git://github.com/DarthFubuMVC/bottle-template.git");
            CreateAlias("fubusln", "git://github.com/DarthFubuMVC/rippletemplate.git");
        }

        public void CreateAlias(string alias, string url)
        {
            var token = AliasFor(alias);
            if (token == null)
            {
                token = new GitAliasToken
                {
                    Url = url,
                    Name = alias
                };

                _aliases.Add(token);
            }
            else
            {
                token.Url = url;
            }
        }

        public GitAliasToken AliasFor(string alias)
        {
            return _aliases.SingleOrDefault(x => x.Name == alias);
        }

        public void RemoveAlias(string alias)
        {
            _aliases.RemoveAll(x => x.Name == alias);
        }
    }

    [XmlType("alias")]
    public class GitAliasToken
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (GitAliasToken)) return false;
            return Equals((GitAliasToken) obj);
        }

        public bool Equals(GitAliasToken other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}