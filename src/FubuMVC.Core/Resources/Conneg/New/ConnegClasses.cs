using System;
using System.Collections.Generic;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Resources.Media;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Conneg.New
{

    public interface IMediaWriter<T>
    {
        void Write(string mimeType, T resource);
        IEnumerable<string> Mimetypes { get; }
    }

    public class HtmlStringWriter<T> : IMediaWriter<T>
    {
        private readonly IOutputWriter _writer;

        public HtmlStringWriter(IOutputWriter writer)
        {
            _writer = writer;
        }

        public void Write(string mimeType, T resource)
        {
            _writer.WriteHtml(resource.ToString());
        }

        public IEnumerable<string> Mimetypes
        {
            get { yield return MimeType.Html.Value; }
        }
    }

    

    public interface IMedia<in T>
    {
        IEnumerable<string> Mimetypes { get; }
        void Write(string mimeType, T resource);
        bool MatchesRequest();
    }

    // TODO -- make this lazy
    public class Media<T> : IMedia<T>
    {
        private readonly IMediaWriter<T> _writer;
        private readonly IConditional _condition;

        public Media(IMediaWriter<T> writer, IConditional condition)
        {
            _writer = writer;
            _condition = condition;
        }

        public IEnumerable<string> Mimetypes
        {
            get { return _writer.Mimetypes; }
        }

        public void Write(string mimeType, T resource)
        {
            _writer.Write(mimeType, resource);
        }

        public bool MatchesRequest()
        {
            return _condition.ShouldExecute();
        }
    }


    public class ReaderChain : Chain<ReaderNode, ReaderChain>
    {
    }

    public abstract class ReaderNode : Node<ReaderNode, ReaderChain>, IContainerModel
    {
        public abstract Type OutputType { get; }

        ObjectDef IContainerModel.ToObjectDef(DiagnosticLevel diagnosticLevel)
        {
            return toObjectDef(diagnosticLevel);
        }

        protected abstract ObjectDef toObjectDef(DiagnosticLevel diagnosticLevel);
    }

    public class WriterChain : Chain<WriterNode, WriterChain>
    {
    }

    public abstract class WriterNode : Node<WriterNode, WriterChain>, IContainerModel
    {
        public abstract Type InputType { get; }

        ObjectDef IContainerModel.ToObjectDef(DiagnosticLevel diagnosticLevel)
        {
            return toObjectDef(diagnosticLevel);
        }

        protected abstract ObjectDef toObjectDef(DiagnosticLevel diagnosticLevel);
    }
}