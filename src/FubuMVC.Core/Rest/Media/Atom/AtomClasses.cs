using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using FubuLocalization;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Rest.Media.Xml;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest.Media.Atom
{
    public interface IFeedDefinition<T>
    {
        IXmlMediaWriter<T> BuildExtensionWriter(IUrlRegistry urls);

        // TODO -- think we need to inject a system date time provider thingie
        void ConfigureFeed(SyndicationFeed feed, DateTime updated);
        void ConfigureItem(SyndicationItem item, IValues<T> values);
    }


    public class Feed<T> : IResourceRegistration, IFeedDefinition<T>
    {
        private readonly IList<Action<SyndicationFeed>> _alterations = new List<Action<SyndicationFeed>>();
        private readonly IList<FeedItem<T>> _maps = new List<FeedItem<T>>();

        public void Title(StringToken title)
        {
            throw new NotImplementedException();
        }

        public void Description(StringToken description)
        {
            throw new NotImplementedException();
        }

        public void Items<TMap>() where TMap : FeedItem<T>
        {
            throw new NotImplementedException();
        }

        public void Items(Action<FeedItem<T>> configure)
        {
            throw new NotImplementedException();
        }

        void IResourceRegistration.Modify(ConnegGraph graph, BehaviorGraph behaviorGraph)
        {
            throw new NotImplementedException();
        }

        IXmlMediaWriter<T> IFeedDefinition<T>.BuildExtensionWriter(IUrlRegistry urls)
        {
            throw new NotImplementedException();
        }

        void IFeedDefinition<T>.ConfigureFeed(SyndicationFeed feed, DateTime updated)
        {
            throw new NotImplementedException();
        }

        void IFeedDefinition<T>.ConfigureItem(SyndicationItem item, IValues<T> values)
        {
            throw new NotImplementedException();
        }
    }

    public static class SyndicationStringExtensions
    {
        public static TextSyndicationContent ToContent(this string text)
        {
            return new TextSyndicationContent(text);
        }
    }
}