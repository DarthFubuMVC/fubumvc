using System;
using System.Collections.Generic;
using System.Linq;

namespace Bottles.Configuration
{
    // Simple notification pattern
    public class BottleConfiguration
    {
        private readonly string _provenance;
        private readonly IList<BottleConfigurationError> _errors = new List<BottleConfigurationError>();

        public BottleConfiguration(string provenance)
        {
            _provenance = provenance;
        }

        public string Provenance
        {
            get { return _provenance; }
        }

        public void RegisterError(BottleConfigurationError error)
        {
            _errors.Fill(error);
        }

        public void RegisterMissingService<T>()
        {
            RegisterMissingService(typeof (T));
        }

        public void RegisterMissingService(Type type)
        {
            RegisterError(new MissingService(type));
        }

        public bool IsValid()
        {
            return !_errors.Any();
        }

        public IEnumerable<BottleConfigurationError> Errors
        {
            get { return _errors; }
        }

        public IEnumerable<MissingService> MissingServices
        {
            get { return _errors.OfType<MissingService>(); }
        }
    }
}