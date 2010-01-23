using System;
using FubuMVC.Core.Registration.Conventions;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    public class OutputDeterminationExpression
    {
        private readonly FubuRegistry _registry;

        public OutputDeterminationExpression(FubuRegistry registry)
        {
            _registry = registry;
        }

        public ActionCallFilterExpression ToJson { get { return output(call => call.Append(new RenderJsonNode(call.OutputType()))); } }

        public ActionCallFilterExpression ToHtml
        {
            get
            {
                return output(call => call.Append(new RenderTextNode<string>
                    {
                        MimeType = MimeType.Html
                    }));
            }
        }

        public ActionCallFilterExpression To(Func<ActionCall, OutputNode> func)
        {
            return output(action =>
            {
                OutputNode node = func(action);
                action.Append(node);
            });
        }

        public ActionCallFilterExpression To<T>() where T : OutputNode, new()
        {
            return output(action => action.Append(new T()));
        }

        private ActionCallFilterExpression output(Action<ActionCall> configure)
        {
            var modification = new ActionCallModification(configure);
            _registry.ApplyConvention(modification);

            modification.Filters.Excludes += call => call.HasOutputBehavior();

            return new ActionCallFilterExpression(modification.Filters);
        }
    }
}