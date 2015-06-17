using System.Collections.Generic;

namespace FubuTransportation.Runtime
{
    public interface IEnvelopeSender
    {
        string Send(Envelope envelope);
    }
}