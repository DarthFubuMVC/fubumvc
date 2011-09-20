using System;
using FubuMVC.Core.Http;
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


    public interface IMediaChoice<T>
    {
        bool Matches(string mimeType);
        void Write(T source);
    }

    public class MediaChoice<T> : IMediaChoice<T>
    {
        private readonly IRequestConditional _conditional;
        private readonly IMediaWriter<T> _writer;
        private readonly IOutputWriter _output;

        public MediaChoice(IRequestConditional conditional, IMediaWriter<T> writer, IOutputWriter output)
        {
            _conditional = conditional;
            _writer = writer;
            _output = output;
        }

        public bool Matches(string mimeType)
        {
            return _conditional.Matches(mimeType);
        }

        public void Write(T source)
        {
            _writer.Write(source, _output);
        }
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
    }

}