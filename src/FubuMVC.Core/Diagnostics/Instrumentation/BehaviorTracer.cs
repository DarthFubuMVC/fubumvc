using System;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public class BehaviorTracer : WrappingBehavior
    {
        private readonly BehaviorNode _node;
        private readonly IChainExecutionLog _log;

        public BehaviorTracer(BehaviorNode node, IChainExecutionLog log)
        {
            _node = node;
            _log = log;
        }

        protected override void invoke(Action action)
        {
            _log.StartSubject(_node);

            try
            {
                action();
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                throw;
            }
            finally
            {
                _log.FinishSubject();
            }
        }
    }
}