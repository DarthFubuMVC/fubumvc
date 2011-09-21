using System;
using System.Collections.Generic;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Media
{
    public interface IMediaWriter<T>
    {
        IEnumerable<string> Mimetypes { get; }
        void Write(IValues<T> source, IOutputWriter writer);
        void Write(T source, IOutputWriter writer);
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

        IEnumerable<string> Mimetypes { get; }
        void Write(IOutputWriter writer);
    }
}