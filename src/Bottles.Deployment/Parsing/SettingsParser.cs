using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using FubuCore.Configuration;

namespace Bottles.Deployment.Parsing
{
    public class SettingsParser
    {
        public static readonly string INVALID_SYNTAX = "Configuration line must be in the form of 'Class.Prop=Value' or 'bottle:<bottle name> <relationship>'";
        private readonly InMemorySettingsData _settings = new InMemorySettingsData();
        private readonly IList<BottleReference> _references = new List<BottleReference>();

        public SettingsParser(string description)
        {
            _settings.Description = description;
        }

        public void ParseText(string text)
        {
            text = text.Trim();
            try
            {
                if (text.StartsWith(ProfileFiles.BottlePrefix))
                {
                    parseBottle(text);
                }
                else if (text.Contains("="))
                {
                    parseProperty(text);
                }
                else
                {
                    throw new SettingsParserException(INVALID_SYNTAX);
                }
            }
            catch (SettingsParserException e)
            {
                e.AppendText(text);
                throw;
            }
        }

        private void parseProperty(string text)
        {
            var parts = text.Split('=');
            var propertyName = parts[0].Trim();

            if (propertyName.IsEmpty())
            {
                throw new SettingsParserException("Missing property name");
            }

            _settings[propertyName] = parts[1].Trim();
        }

        private void parseBottle(string text)
        {
            var bottleText = text.Substring(ProfileFiles.BottlePrefix.Length).Trim();
            var values = StringTokenizer.Tokenize(bottleText);
            var reference = new BottleReference(){
                Name = values.First()
            };

            if (values.Count() == 2)
            {
                reference.Relationship = values.Skip(1).First();
            }

            _references.Add(reference);
        }

        public ISettingsData Settings
        {
            get { return _settings; }
        }

        public IEnumerable<BottleReference> References
        {
            get { return _references; }
        }
    }
}