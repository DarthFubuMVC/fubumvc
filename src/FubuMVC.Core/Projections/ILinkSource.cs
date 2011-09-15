using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace FubuMVC.Core.Projections
{
    public interface ILinkSource
    {
        IEnumerable<SyndicationLink> LinksFor(IProjectionTarget target);
    }
}