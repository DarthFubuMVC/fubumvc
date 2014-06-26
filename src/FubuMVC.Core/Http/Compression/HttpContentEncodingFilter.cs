using FubuCore.Binding;
using FubuCore.Descriptions;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Compression
{
    public class HttpContentEncodingFilter : IBehaviorInvocationFilter, DescribesItself
    {
        private readonly IHttpContentEncoders _encoders;

        public HttpContentEncodingFilter(IHttpContentEncoders encoders)
        {
            _encoders = encoders;
        }

        public DoNext Filter(ServiceArguments arguments)
        {
            if (arguments.Has(typeof (Latch))) return DoNext.Stop;


            var request = arguments.Get<IHttpRequest>();
            if (!request.HasHeader(HttpRequestHeaders.AcceptEncoding)) return DoNext.Continue;

            var acceptEncoding = request
                .GetSingleHeader(HttpRequestHeaders.AcceptEncoding);


            var encoding = _encoders.MatchFor(acceptEncoding);
            var writer = arguments.Get<IHttpResponse>();
            writer.AppendHeader(HttpRequestHeaders.ContentEncoding, encoding.MatchingEncoding.Value);

            writer.UseEncoding(encoding);

            arguments.Set(typeof (Latch), new Latch());

            return DoNext.Continue;
        }

        public void Describe(Description description)
        {
            description.AddList("Encoders", _encoders.Encodings);
        }
    }

    public class Latch
    {
    }
}