using System;
using System.Collections.Generic;
using System.Linq;

namespace FubuCore.CommandLine
{
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
                                               (Mandatories.Union(Flags).Select(x => x.ToUsageDescription())).Join(" "));
            }
        }

        public bool ArgumentsMatch(IEnumerable<ITokenHandler> actuals)
        {
            return Mandatories.All(x => actuals.Contains(x));
        }

        public bool AllFlagsAreValid(IEnumerable<ITokenHandler> actuals)
        {
            var flags = actuals.Where(x => !(x is Argument));

            throw new NotImplementedException();
        }
    }
}