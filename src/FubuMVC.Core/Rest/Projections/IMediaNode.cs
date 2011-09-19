using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace FubuMVC.Core.Rest.Projections
{
    public interface IMediaNode
    {
        IMediaNode AddChild(string name);
        void SetAttribute(string name, object value);

        void WriteLinks(IEnumerable<SyndicationLink> links);

    }
}