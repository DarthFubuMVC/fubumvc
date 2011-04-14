using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FubuCore.CommandLine;
using FubuCore.Configuration;
using System.Linq;
using FubuCore;

namespace Bottles.Deployment.Parsing
{
    [Serializable]
    public class SettingsParserException : Exception
    {
        private string _message;

        public SettingsParserException(string message)
        {
            _message = message;
        }

        protected SettingsParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string Message
        {
            get { return _message; }
        }

        public void AppendText(string text)
        {
            _message += "\n" + text;
        }
    }

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
                if (text.StartsWith("bottle:"))
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
            var bottleText = text.Substring(7).Trim();
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