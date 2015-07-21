using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public class OrderedAssignment
    {
        private readonly Uri _subject;
        private readonly ITransportPeer[] _peers;
        private int _index;
        private readonly TaskCompletionSource<ITransportPeer> _completion;

        public OrderedAssignment(Uri subject, IEnumerable<ITransportPeer> peers)
        {
            _subject = subject;
            _peers = peers.ToArray();
            _index = 0;

            _completion = new TaskCompletionSource<ITransportPeer>(TaskCreationOptions.AttachedToParent);
        }

        public Task<ITransportPeer> SelectOwner()
        {
            tryToSelect();

            return _completion.Task;
        }

        private Task tryToSelect()
        {
            var transportPeer = _peers[_index++];
            return transportPeer.TakeOwnership(_subject).ContinueWith(t => {
                if (shouldContinue(t))
                {
                    tryToSelect();
                }
                else if (t.Result == OwnershipStatus.AlreadyOwned || t.Result == OwnershipStatus.OwnershipActivated)
                {
                    _completion.SetResult(transportPeer);
                }
                else
                {
                    _completion.SetResult(null);
                }
            });
        }

        private bool shouldContinue(Task<OwnershipStatus> task)
        {
            if (_index >= _peers.Length) return false;

            if (task.IsFaulted) return true;

            if (task.Result == OwnershipStatus.OwnershipActivated || task.Result == OwnershipStatus.AlreadyOwned) return false;

            return true;
        }
    }
}