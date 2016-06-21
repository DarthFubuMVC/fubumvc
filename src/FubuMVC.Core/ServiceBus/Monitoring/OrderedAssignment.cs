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

        public OrderedAssignment(Uri subject, IEnumerable<ITransportPeer> peers)
        {
            _subject = subject;
            _peers = peers.ToArray();
            _index = 0;
        }

        public async Task<ITransportPeer> SelectOwner()
        {
            return await tryToSelect().ConfigureAwait(false);
        }

        private async Task<ITransportPeer> tryToSelect()
        {
            var transportPeer = _peers[_index++];

            try
            {
                var status = await transportPeer.TakeOwnership(_subject).ConfigureAwait(false);

                if (status == OwnershipStatus.AlreadyOwned || status == OwnershipStatus.OwnershipActivated)
                {
                    return transportPeer;
                }
            }
            catch (Exception)
            {
                // TODO -- maybe log this one?
            }

            if (_index >= _peers.Length) return null;

            return await tryToSelect().ConfigureAwait(false);
        }
    }
}