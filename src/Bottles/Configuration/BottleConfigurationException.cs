using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuCore;

namespace Bottles.Configuration
{
    public class BottleConfigurationException : Exception
    {
        private readonly string _provenance;
        private readonly IEnumerable<BottleConfigurationError> _errors;

        public BottleConfigurationException(string provenance, IEnumerable<BottleConfigurationError> errors)
        {
            _provenance = provenance;
            _errors = errors;
        }

        public override string Message
        {
            get
            {
                var message = new StringBuilder("Bottle Configuration Error: {0}".ToFormat(_provenance)).AppendLine();
                Errors.Select(x => x.ToString()).Each(x => message.AppendLine(x));

                return message.ToString();
            }
        }

        public IEnumerable<BottleConfigurationError> Errors
        {
            get { return _errors; }
        }

        public string Provenance
        {
            get { return _provenance; }
        }

        public IEnumerable<MissingService> MissingPlugins
        {
            get { return _errors.OfType<MissingService>(); }
        }
    }
}