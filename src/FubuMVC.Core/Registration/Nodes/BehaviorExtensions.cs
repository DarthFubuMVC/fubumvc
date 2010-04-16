using System.Collections.Generic;

namespace FubuMVC.Core.Registration.Nodes
{
    public static class BehaviorExtensions
    {
        public static void Configure(this List<IConfigurationAction> actions, BehaviorGraph graph)
        {
            actions.Each(x => x.Configure(graph));
        }

        /// <summary>
        /// Returns true if the <paramref name="action"/> is defined by the FubuMVC framework itself. Returns false if the <paramref name="action"/> is defined in user code.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool IsInternalFubuAction(this ActionCall action)
        {
            return action.HandlerType.Assembly.Equals(typeof(ActionCall).Assembly);
        }
    }
}