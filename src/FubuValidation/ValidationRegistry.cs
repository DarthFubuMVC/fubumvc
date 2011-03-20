using System;
using System.Collections.Generic;
namespace FubuValidation
{
    public class ValidationRegistry
    {
        private readonly List<IValidationSource> _sources = new List<IValidationSource>();

        public ValidationRegistry()
        {
        }

        public ValidationRegistry(Action<ValidationRegistry> configure)
            : this()
        {
            configure(this);
        }

        public IEnumerable<IValidationSource> GetConfiguredSources()
        {
            return _sources.ToArray();
        }
    }
}