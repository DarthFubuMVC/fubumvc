using System;

namespace FubuTransportation.Monitoring
{
    public class TakeOwnershipResponse
    {
        public Uri Subject { get; set; }
        public OwnershipStatus Status { get; set; }
        public string NodeId { get; set; }
    }
}