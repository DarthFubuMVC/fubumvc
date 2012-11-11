using System;
using System.Linq;
using System.Collections.Generic;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    public class DefaultOutputPolicy : Policy
    {
        public DefaultOutputPolicy()
        {
            Where.IsNotPartial();
            Where.ChainMatches(x => x.HasResourceType() && !x.HasOutput());
            ModifyBy(chain => chain.ApplyConneg());
        }

    }
}