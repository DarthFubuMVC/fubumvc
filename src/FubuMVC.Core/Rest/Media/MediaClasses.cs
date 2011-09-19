using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Media.Projections;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaReader<T>
    {
        T Retrieve(CurrentRequest request);
    }

    public interface IMediaWriter<T>
    {
        void Write();
    }


    public interface IMedia<T>
    {
        void Write(IValues<T> source, IOutputWriter writer);
        void Write(T source, IOutputWriter writer);
    }

    // this to be consumed by IMediaReader<T>
    public interface IValueSource<T>
    {
        IValues<T> FindValues();
    }


    public interface IMediaChoice
    {
        
    }

    // TODO -- test this with an integrated test.
    // Don't really care if it's tested with a bunch of mocks.
    public class Media<T> : IMedia<T>
    {
        private readonly IMediaDocument _document;
        private readonly ILinkSource<T> _links;
        private readonly IUrlRegistry _urls;
        private readonly IValueProjection<T> _projection;

        public Media(IMediaDocument document, ILinkSource<T> links, IUrlRegistry urls, IValueProjection<T> projection)
        {
            _document = document;
            _links = links;
            _urls = urls;
            _projection = projection;
        }

        public void Write(IValues<T> source, IOutputWriter writer)
        {
            var links = _links.LinksFor(source, _urls);
            var topNode = _document.Root;
            topNode.WriteLinks(links);

            _projection.WriteValue(source, topNode);

            _document.Write(writer);
        }

        public void Write(T source, IOutputWriter writer)
        {
            Write(new SimpleValues<T>(source), writer);
        }
    }

    public class MediaDefinition<T>
    {
        public ObjectDef ToObjectDef()
        {
            throw new NotImplementedException();
        }
    }

    public interface IMediaDocument
    {
        IMediaNode Root { get; }
        void Write(IOutputWriter writer);
    }




    //public interface IXmlMedia<T>
    //{
    //    XmlDocument CreateDocument(IValues<T> source, IEnumerable<SyndicationLink> links);
    //}

    //public interface IMediaStreamDefinition<T>
    //{
    //    void Start(DateTime currentTime, IEnumerable<SyndicationLink> links);
    //    void AddItem(IValues<T> source, IEnumerable<SyndicationLink> links);
    //    void Write(IOutputWriter writer);
    //}

    /*
     * In the scanning for conneg node, look for types and try to hook up the media definition stuff
     * 
     * 
     * 
     * 
     */
}