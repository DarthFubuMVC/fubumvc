using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace FubuMVC.Core.ServiceBus.Runtime.Headers
{
    public interface IHeaders
    {
        [IndexerName("Items")] // VB.Net guys might wanna use this.  Stranger things have happened
        string this[string key] { get; set; }

        IEnumerable<string> Keys();

        NameValueCollection ToNameValues();

        bool Has(string key);
        void Remove(string key);
    }

}