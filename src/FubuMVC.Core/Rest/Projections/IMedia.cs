using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Projections
{
    // put this into a ReST namespace

    public interface IMediaDefinition<T>
    {
        void Write(IValueSource<T> source, IEnumerable<SyndicationLink> links, IOutputWriter writer);
        bool MatchesMimetype(string mimetype);
    }

    public interface IXmlMedia<T>
    {
        XmlDocument CreateDocument(IValueSource<T> source, IEnumerable<SyndicationLink> links);
    }

    public interface IMediaStreamDefinition<T>
    {
        void Start(DateTime currentTime, IEnumerable<SyndicationLink> links);
        void AddItem(IValueSource<T> source, IEnumerable<SyndicationLink> links);
        void Write(IOutputWriter writer);
    }

    /*
     * In the scanning for conneg node, look for types and try to hook up the media definition stuff
     * 
     * 
     * 
     * 
     */


}