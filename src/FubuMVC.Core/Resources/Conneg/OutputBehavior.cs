using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Conditionals;

namespace FubuMVC.Core.Resources.Conneg
{
    public class OutputBehavior<T> : BasicBehavior where T : class
    {
        private readonly IFubuRequest _request;
        private readonly IOutputWriter _writer;
        private readonly IEnumerable<IMedia<T>> _media;
        private readonly ILogger _logger;

        public OutputBehavior(IFubuRequest request, IOutputWriter writer, IEnumerable<IMedia<T>> media, ILogger logger) : base(PartialBehavior.Executes)
        {
            _request = request;
            _writer = writer;
            _media = media;
            _logger = logger;
        }

        protected override void afterInsideBehavior()
        {
            Write();
        }

        public void Write()
        {
            

            var mimeTypes = _request.Get<CurrentMimeType>();
            var media = SelectMedia(mimeTypes);

            if (media == null)
            {
                // TODO -- better error message?
                _writer.WriteResponseCode(HttpStatusCode.NotAcceptable);
                _writer.Write(MimeType.Text, "406:  Not acceptable");
            }
            else
            {
                var resource = _request.Get<T>();
                var outputMimetype = mimeTypes.SelectFirstMatching(media.Mimetypes);
                media.Write(outputMimetype, resource);
            }

            WriteHeaders();
        }

        public void WriteHeaders()
        {
            _request.Find<IHaveHeaders>()
                .SelectMany(x => x.Headers)
                .Each(x => x.Write(_writer));
        }

        public virtual IMedia<T> SelectMedia(CurrentMimeType mimeTypes)
        {
            foreach (var acceptType in mimeTypes.AcceptTypes)
            {
                var candidates = _media.Where(x => x.Mimetypes.Contains(acceptType));
                if (candidates.Any())
                {
                    var writer = candidates.FirstOrDefault(x => x.MatchesRequest());
                    if (writer != null)
                    {
                        _logger.DebugMessage(() => new WriterChoice(acceptType, writer, writer.Condition));
                        return writer;
                    }
                    
                    _logger.DebugMessage(() => NoWritersMatch.For(acceptType, candidates));
                }
            }

            if (mimeTypes.AcceptsAny())
            {
                var media = _media.FirstOrDefault(x => x.MatchesRequest());
                _logger.DebugMessage(() => new WriterChoice(MimeType.Any.Value, media, media.Condition));

                return media;
            }

            return null;
        }

        public IEnumerable<IMedia<T>> Media
        {
            get { return _media; }
        }
    }

    public class NoMatchingWriter : LogRecord, DescribesItself
    {
        private readonly CurrentMimeType _mimeType;

        public NoMatchingWriter(CurrentMimeType mimeType)
        {
            _mimeType = mimeType;
        }

        public void Describe(Description description)
        {
            description.Title = "No writers matched the runtime conditions and accept-type: " + _mimeType.AcceptTypes;
        }
    }

    public class WriterChoice : LogRecord, DescribesItself
    {
        private readonly string _mimeType;
        private readonly Description _writer;
        private readonly Description _condition;

        public WriterChoice(string mimeType, object writer, IConditional condition)
        {
            _mimeType = mimeType;
            _writer = Description.For(writer);
            _condition = Description.For(condition);
        }

        public void Describe(Description description)
        {
            description.Title = "Selected writer '{0}'".ToFormat(_writer.Title);

            if (_writer.HasExplicitShortDescription())
            {
                description.Properties["Writer"] = _writer.ShortDescription;
            }
            
            description.Properties["Mimetype"] = _mimeType;
            description.Properties["Condition"] = _condition.Title;
        }
    }

    public class NoWritersMatch : LogRecord, DescribesItself
    {
        public static NoWritersMatch For<T>(string acceptType, IEnumerable<IMedia<T>> candidates)
        {
            var match = new NoWritersMatch{
                WriterList = candidates.Select(writer =>
                {
                    var title = Description.For(writer).Title;
                    var condition = Description.For(writer.Condition).Title;

                    return "{0} ({1})".ToFormat(title, condition);
                }).Join(", ")
            };

            match.MimeType = acceptType;

            return match;
        }

        public string MimeType { get; set; }
        public string WriterList { get; set; }

        public void Describe(Description description)
        {
            description.Title = "Conditions were not met for any available writers for mimetype " + MimeType;
            description.ShortDescription = "Candidate writers:  " + WriterList;
        }
    }
}