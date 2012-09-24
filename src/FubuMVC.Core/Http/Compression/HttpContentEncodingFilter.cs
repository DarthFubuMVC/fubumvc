using FubuCore.Binding;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Compression
{
    public class HttpContentEncodingFilter : IBehaviorInvocationFilter
    {
        private readonly IHttpContentEncoders _encoders;

        public HttpContentEncodingFilter(IHttpContentEncoders encoders)
        {
            _encoders = encoders;
        }

        public DoNext Filter(ServiceArguments arguments)
        {
            if (arguments.Has(typeof(Latch))) return DoNext.Stop;

            arguments
                .Get<IRequestData>()
                .ValuesFor(RequestDataSource.Header)
                .Value(HttpRequestHeaders.AcceptEncoding, x =>
                {
                    var encoding = _encoders.MatchFor(x.RawValue as string);
                    var writer = arguments.Get<IHttpWriter>();

                    writer.AppendHeader(HttpRequestHeaders.ContentEncoding, encoding.MatchingEncoding.Value);
                    writer.UseEncoding(encoding);
                });

            arguments.Set(typeof(Latch), new Latch());

            return DoNext.Continue;
        }
    }

    public class Latch{}
}