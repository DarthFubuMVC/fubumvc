using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.HelloWorld.Controllers.NonAjaxOnly
{
    public class NonAjaxConditionalPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(x => x.FirstCall().OutputType().CanBeCastTo<NonAjaxModel>())
                .Each(chain =>
                          {
                              chain.Outputs.First().ConditionallyRunIf<CheckForQueryString>();
                              //chain.FirstCall().AddAfter(new Wrapper(typeof (MiddleWare)));
                              chain.FirstCall().AddAfter(new Wrapper(typeof(MiddleWare))
                                  .ConditionallyRunIf<CurrentRequest>(x => x.RawUrl.Contains("render=false")));
                               var renderJsonNode = new RenderJsonNode(typeof(NonAjaxModel));
                               chain.AddToEnd(renderJsonNode.ConditionallyRunIf<IsAjaxRequest>());
                          });
        }
    }
    public class CheckForQueryString : ConditionalBehavior<IFubuRequest>
    {
        public CheckForQueryString(IActionBehavior innerBehavior, IFubuRequest context)
            : base(innerBehavior, context, x =>
                                               {
                                                   return x.Get<CurrentRequest>()
                                                       .RawUrl.Contains("render=true");
                                               })
        {
        }
    }

    public class MiddleWare : BasicBehavior
    {
        private readonly IOutputWriter _writer;

        public MiddleWare(IOutputWriter writer) : base(PartialBehavior.Executes)
        {
            _writer = writer;
        }
        protected override DoNext performInvoke()
        {
            _writer.Write("text/html", "Hello!");
            return DoNext.Continue;
        }
    }
}