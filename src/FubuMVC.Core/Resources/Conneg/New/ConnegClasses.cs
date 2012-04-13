using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Behaviors.Conditional;
using FubuMVC.Core.Http;
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

    // TODO -- Runtime tracing
    public class OutputBehavior<T> : BasicBehavior where T : class
    {
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;
        private readonly IEnumerable<IMedia<T>> _media;

        public OutputBehavior(IFubuRequest request, IOutputWriter writer, IEnumerable<IMedia<T>> media) : base(PartialBehavior.Executes)
        {
            _request = request;
            _writer = writer;
            _media = media;
        }

        protected override DoNext performInvoke()
        {
            Write();

            return DoNext.Continue;
        }

        public void Write()
        {
            var mimeTypes = _request.Get<CurrentMimeType>();
            var media = SelectMedia(mimeTypes);

            if (media == null)
            {
                _writer.WriteResponseCode(HttpStatusCode.NotAcceptable);
            }
            else
            {
                var resource = _request.Get<T>();
                var outputMimetype = mimeTypes.SelectFirstMatching(media.Mimetypes);
                media.Write(outputMimetype, resource);
            }
        }

        public IMedia<T> SelectMedia(CurrentMimeType mimeTypes)
        {
            foreach (var acceptType in mimeTypes.AcceptTypes)
            {
                var media = _media.FirstOrDefault(x => x.Mimetypes.Contains(acceptType) && x.MatchesRequest());
                if (media != null) return media;
            }

            if (mimeTypes.AcceptsAny() && InsideBehavior == null)
            {
                return _media.FirstOrDefault(x => x.MatchesRequest());
            }

            return null;
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