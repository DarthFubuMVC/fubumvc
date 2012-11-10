using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Registration.Conventions
{
    [Policy]
    public class JsonMessageInputConvention : Policy
    {
        public JsonMessageInputConvention()
        {
            Where.IsNotPartial();
            Where.ResourceTypeImplements<JsonMessage>().Or.InputTypeImplements<JsonMessage>();
            Conneg.MakeAsymmetricJson();
        }
    }
}