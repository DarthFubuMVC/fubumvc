using System.Collections.Generic;
using Bottles.Diagnostics;

namespace Bottles.Configuration
{
    public class AssertBottleConfiguration : IActivator
    {
        private readonly string _provenance;
        private readonly IEnumerable<IBottleConfigurationRule> _rules;

        public AssertBottleConfiguration(string provenance, IEnumerable<IBottleConfigurationRule> rules)
        {
            _provenance = provenance;
            _rules = rules;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            var configuration = new BottleConfiguration(_provenance);
            _rules.Each(r => r.Evaluate(configuration));

            if (!configuration.IsValid())
            {
                throw new BottleConfigurationException(_provenance, configuration.Errors);
            }
        }
    }
}