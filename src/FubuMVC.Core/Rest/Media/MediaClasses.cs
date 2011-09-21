using System;
using System.Collections.Generic;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Rest.Media.Formatters;
using FubuMVC.Core.Runtime;
using System.Linq;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaWriter<T>
    {
        void Write(IValues<T> source, IOutputWriter writer);
        void Write(T source, IOutputWriter writer);

        IEnumerable<string> Mimetypes { get; }
    }


    public class MediaDefinition<T>
    {
        

        // Maybe multiples
        public ObjectDef ToObjectDef()
        {
            throw new NotImplementedException();
        }
    }

    public interface IMediaDocument
    {
        IMediaNode Root { get; }
        void Write(IOutputWriter writer);

        IEnumerable<string> Mimetypes { get; }
    }

}