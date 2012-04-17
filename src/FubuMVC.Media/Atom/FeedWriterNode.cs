using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Conneg.New;

namespace FubuMVC.Media.Atom
{
    public class FeedWriterNode<T> : WriterNode
    {
        private readonly IFeedDefinition<T> _feed;

        public FeedWriterNode(IFeedDefinition<T> feed, Type modelType)
        {
            _feed = feed;

            InputType = modelType;
        }

        public IFeedDefinition<T> Feed
        {
            get { return _feed; }
        }

        public Type InputType { get; set; }

        public override Type ResourceType
        {
            get { return typeof (IEnumerable<T>); }
        }

        // TODO -- UT this
        public override IEnumerable<string> Mimetypes
        {
            get { yield return _feed.ContentType; }
        }

        protected override ObjectDef toWriterDef()
        {
            var objectDef = new ObjectDef(typeof (FeedWriter<T>));
            objectDef.DependencyByValue(typeof (IFeedDefinition<T>), _feed);

            return objectDef;
        }
    }
}