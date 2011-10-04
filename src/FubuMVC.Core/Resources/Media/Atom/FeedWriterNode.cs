using System;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Resources.Media.Atom
{
    public enum FeedSourceType
    {
        direct,
        enumerable
    }

    public class FeedWriterNode<T> : IMediaWriterNode
    {
        private readonly IFeedDefinition<T> _feed;
        // IFeedSource<T>
        public FeedWriterNode(IFeedDefinition<T> feed, FeedSourceType sourceType, Type modelType)
        {
            _feed = feed;
            FeedSourceType = sourceType == Atom.FeedSourceType.direct
                                 ? typeof (DirectFeedSource<,>).MakeGenericType(modelType, typeof (T))
                                 : typeof (EnumerableFeedSource<,>).MakeGenericType(modelType, typeof (T));

            InputType = modelType;
        }

        public IFeedDefinition<T> Feed
        {
            get { return _feed; }
        }

        public Type FeedSourceType
        {
            get; private set;
        }

        public ObjectDef ToObjectDef(DiagnosticLevel level)
        {
            var objectDef = new ObjectDef(typeof (FeedWriter<T>));
            objectDef.DependencyByType(typeof (IFeedSource<T>), FeedSourceType);
            objectDef.DependencyByValue(typeof(IFeedDefinition<T>), _feed);

            return objectDef;
        }

        public Type InputType
        {
            get; set;
        }
    }
}