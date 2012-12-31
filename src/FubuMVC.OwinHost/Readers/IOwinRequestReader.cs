using System.Collections.Generic;

namespace FubuMVC.OwinHost.Readers
{
    public interface IOwinRequestReader
    {
        void Read(IDictionary<string, object> environment);
    }
}