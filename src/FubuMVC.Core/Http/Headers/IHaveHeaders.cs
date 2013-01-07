using System.Collections.Generic;

namespace FubuMVC.Core.Http.Headers
{
    /// <summary>
    /// Marker interface for a resource type that also sets http header values
    /// </summary>
    public interface IHaveHeaders
    {
        IEnumerable<Header> Headers { get; }
    }
}