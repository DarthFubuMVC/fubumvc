using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Packaging;
using System.Linq;

namespace FubuMVC.Core.Conneg
{
    // Use current request.ContentType
    // application/x-www-form-urlencoded; charset=UTF-8
    // probably want CurrentRequest.RequestedMimeType()
    /*
     * 
     * TODO --
     * 1.) Register the serialization classes
     * 2.) Register the baseline media processor
     * 
     * 
     * 3.) 
     * 
     * New thing in ObjectResolver?  IRequestMediaHandler?  One is IModelBinderCache, another is media?
     * 
     */

    // Extend FubuRegistry for this stuff
    public class ConnegBehaviorConvention : IConfigurationAction
    {
        private readonly Func<BehaviorChain, bool> _filter;

        public ConnegBehaviorConvention(Func<BehaviorChain, bool> filter)
        {
            _filter = filter;
        }
        
        public void Configure(BehaviorGraph graph)
        {
            graph.Behaviors.Where(_filter).Each(attachMediaHandling);
        }

        private static void attachMediaHandling(BehaviorChain chain)
        {
            var firstAction = chain.FirstCall();
            if (firstAction == null) return;

            var inputType = chain.InputType();
            var outputType = chain.Calls.Where(x => x.HasOutput).Select(x => x.OutputType()).LastOrDefault();

            throw new NotImplementedException();
        }
    }

// need a new MediaNode

    public class ConnegBehavior : IActionBehavior
    {
        private readonly IFubuRequest _request;
        private readonly IActionBehavior _inner;
        private readonly IConnegInputHandler _inputHandler;
        private readonly IConnegOutputHandler _outputHandler;

        public ConnegBehavior(IFubuRequest request, IActionBehavior inner, IConnegInputHandler inputHandler, IConnegOutputHandler outputHandler)
        {
            _request = request;
            _inner = inner;
            _inputHandler = inputHandler;
            _outputHandler = outputHandler;
        }

        private void execute(Action innerInvocation)
        {
            var currentRequest = _request.Get<CurrentRequest>();
            _inputHandler.ReadInput(currentRequest, _request);

            innerInvocation();

            _outputHandler.WriteOutput(currentRequest, _request);
        }

        public void Invoke()
        {
            execute(() => _inner.Invoke());
        }

        public void InvokePartial()
        {
            execute(() => _inner.InvokePartial());
        }

        public interface IConnegInputHandler
        {
            void ReadInput(CurrentRequest currentRequest, IFubuRequest request);
        }

        public class ConnegInputHandler<T> : IConnegInputHandler where T : class
        {
            private readonly IMediaProcessor<T> _processor;

            public ConnegInputHandler(IMediaProcessor<T> processor)
            {
                _processor = processor;
            }

            public void ReadInput(CurrentRequest currentRequest, IFubuRequest request)
            {
                var input = _processor.Retrieve(currentRequest);
                request.Set(input);
            }
        }

        public class ConnegOutputHandler<T> : IConnegOutputHandler where T : class
        {
            private readonly IMediaProcessor<T> _processor;

            public ConnegOutputHandler(IMediaProcessor<T> processor)
            {
                _processor = processor;
            }

            public void WriteOutput(CurrentRequest currentRequest, IFubuRequest request)
            {
                var output = request.Get<T>();
                _processor.Write(output, currentRequest);
            }
        }

        public class NulloConnegHandler : IConnegInputHandler, IConnegOutputHandler
        {
            public void ReadInput(CurrentRequest currentRequest, IFubuRequest request)
            {
                // do nothing
            }

            public void WriteOutput(CurrentRequest currentRequest, IFubuRequest request)
            {
                // do nothing
            }
        }

        public interface IConnegOutputHandler
        {
            void WriteOutput(CurrentRequest currentRequest, IFubuRequest request);
        }
    }


    public interface IMediaProcessor<T>
    {
        T Retrieve(CurrentRequest request);
        void Write(T target, CurrentRequest request);
    }

    [Serializable]
    public class MediaProcessingException : Exception
    {
        public MediaProcessingException(string message) : base(message)
        {
        }

        protected MediaProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class MediaProcessor<T> : IMediaProcessor<T>
    {
        private readonly IEnumerable<IFormatter> _formatters;

        public MediaProcessor(IEnumerable<IFormatter> formatters)
        {
            _formatters = formatters;
        }

        public T Retrieve(CurrentRequest request)
        {
            return findFormatter(request).Read<T>();
        }

        private IFormatter findFormatter(CurrentRequest request)
        {
            var formatter = _formatters.FirstOrDefault(x => x.Matches(request));
            if (formatter == null)
            {
                throw new MediaProcessingException("Could not determine a formatter for this request");
            }

            return formatter;
        }

        public void Write(T target, CurrentRequest request)
        {
            findFormatter(request).Write(target);
        }
    }

    

    public interface IFormatter
    {
        T Read<T>();
        void Write<T>(T target);
        bool Matches(CurrentRequest request);
    }

    public class XmlFormatter : IFormatter
    {
        private readonly IStreamingData _streaming;

        public XmlFormatter(IStreamingData streaming)
        {
            _streaming = streaming;
        }

        public T Read<T>()
        {
            var serializer = new XmlSerializer(typeof (T));
            return (T) serializer.Deserialize(_streaming.Input);
        }

        public void Write<T>(T target)
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(_streaming.Output, target);
        }

        public bool Matches(CurrentRequest request)
        {
            return request.MatchesOneOfTheseMimeTypes("text/xml", "application/xml");
        }
    }

    public class JsonFormatter : IFormatter
    {
        private readonly IJsonWriter _writer;
        private readonly IJsonReader _reader;

        public JsonFormatter(IJsonWriter writer, IJsonReader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public T Read<T>()
        {
            return _reader.Read<T>();
        }

        public void Write<T>(T target)
        {
            _writer.Write(target);
        }

        public bool Matches(CurrentRequest request)
        {
            return request.MatchesOneOfTheseMimeTypes("application/json", "text/json");
        }
    }

    
}