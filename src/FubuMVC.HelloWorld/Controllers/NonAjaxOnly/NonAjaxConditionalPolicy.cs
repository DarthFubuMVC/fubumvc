using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.HelloWorld.Controllers.NonAjaxOnly
{
    public class NonAjaxConditionalPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.FirstCall().OutputType().CanBeCastTo<NonAjaxModel>())
                .Each(b =>
                          {
                              b.Outputs.First().ConditionallyRunIf<IsNotAjaxRequest>();

                              var renderJsonNode = new RenderJsonNode(typeof(NonAjaxModel));
                              renderJsonNode.ConditionallyRunIf<IsAjaxRequest>();

                              b.AddToEnd(renderJsonNode);
                          });
        }
    }
}