using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaNode
    {
        IMediaNode AddChild(string name);
        void SetAttribute(string name, object value);

        void WriteLinks(IEnumerable<SyndicationLink> links);

    }
}