using System.Collections.Generic;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaNode
    {
        IMediaNode AddChild(string name);
        void SetAttribute(string name, object value);

        void WriteLinks(IEnumerable<Link> links);
    }
}