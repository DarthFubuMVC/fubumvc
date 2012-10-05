    
using System;
using System.Linq;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.View
{
    public static class OutputNodeExtensions
    {
        public static ViewNode AddView(this OutputNode outputNode, IViewToken view, Type conditionType = null)
        {
            var node = new ViewNode(view);
            if (conditionType != null && conditionType != typeof(Always))
            {
                node.Condition(conditionType);
            }

            outputNode.Writers.AddToEnd(node);

            return node;
        }

        public static bool HasView(this OutputNode outputNode, Type conditionalType)
        {
            return outputNode.Writers.OfType<ViewNode>().Any(x => x.ConditionType == conditionalType);
        }
    }
}