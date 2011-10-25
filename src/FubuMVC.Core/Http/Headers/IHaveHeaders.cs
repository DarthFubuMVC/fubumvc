using System.Collections.Generic;

namespace FubuMVC.Core.Http.Headers
{
    public interface IHaveHeaders
    {
        IEnumerable<Header> Headers { get; }
    }
}