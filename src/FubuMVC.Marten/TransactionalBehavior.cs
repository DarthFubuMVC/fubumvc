using System;
using System.Threading.Tasks;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Marten
{
    public class TransactionalBehavior : WrappingBehavior
    {
        private readonly ISessionBoundary _session;

        public TransactionalBehavior(ISessionBoundary session)
        {
            _session = session;
        }

        protected override async Task invoke(Func<Task> func)
        {
            try
            {
                await func().ConfigureAwait(false);
                await _session.SaveChanges().ConfigureAwait(false);
            }
            catch (Exception)
            {
                _session.Dispose();

                throw;
            }
        }
    }
}