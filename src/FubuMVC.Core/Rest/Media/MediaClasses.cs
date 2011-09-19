using System;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaReader<T>
    {
        T Retrieve(CurrentRequest request);
    }

    public interface IMediaWriter<T>
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