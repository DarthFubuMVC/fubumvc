using System.Collections.Generic;

namespace FubuMVC.Core.Http.Owin.Readers
{
    public interface IOwinRequestReader
    {
        void Read(IDictionary<string, object> environment);
    }
}