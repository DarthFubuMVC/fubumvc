using System.ComponentModel;
using System.Linq;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Policies;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Compression
{
    [Description("Apply Content Compression")]
    public class ApplyContentCompression : IChainModification, DescribesItself
    {
        public readonly static HttpContentEncodingFilter DefaultFilter = new HttpContentEncodingFilter(new HttpContentEncoders(new IHttpContentEncoding[]{new GZipHttpContentEncoding(), new DeflateHttpContentEncoding()}));

        private readonly IBehaviorInvocationFilter _filter;

        public ApplyContentCompression()
        {
            _filter = DefaultFilter;
        }

        public ApplyContentCompression(params IHttpContentEncoding[] encodings)
        {
            _filter = encodings.Any() ? new HttpContentEncodingFilter(new HttpContentEncoders(encodings)) : DefaultFilter;
        }

        public void Modify(BehaviorChain chain)
        {
            if (chain.Calls.Any(x => x.HasAttribute<DoNotCompressAttribute>()))
            {
                return;
            }

            chain.AddFilter(_filter);
        }

        void DescribesItself.Describe(Description description)
        {
            description.Title = "Adds Http content compression";
            description.AddChild("Filter", _filter);
        }
    }
}