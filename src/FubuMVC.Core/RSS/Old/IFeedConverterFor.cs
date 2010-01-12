using System.ServiceModel.Syndication;

namespace FubuMVC.Core.Runtime
{
    public interface IFeedConverterFor<MODEL>
    {
        bool TryConvertModel(MODEL model, out SyndicationFeed syndicationFeed);
    }

    public class DefaultFeedConverter<T> : IFeedConverterFor<T>
    {
        public bool TryConvertModel(T model, out SyndicationFeed syndicationFeed)
        {
            syndicationFeed = new SyndicationFeed();
            return false;
        }
    }
}