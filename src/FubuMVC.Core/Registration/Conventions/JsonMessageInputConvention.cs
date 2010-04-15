using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    public class JsonMessageInputConvention : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Actions().Where(x => x.InputType().CanBeCastTo<JsonMessage>()).ToList().Each(x =>
            {
                var inputType = x.InputType();
                var deserialization = new DeserializeJsonNode(inputType);

                x.AddBefore(deserialization);
            });
        }
    }
}