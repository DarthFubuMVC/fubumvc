using System;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Dates;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Urls;
using FubuMVC.Diagnostics.Chains;
using FubuMVC.Diagnostics.Endpoints;

namespace FubuMVC.Diagnostics.Runtime.Tracing
{
    public class RequestLogBuilder : IRequestLogBuilder
    {
        private readonly IUrlRegistry _urls;
        private readonly ISystemTime _systemTime;
        private readonly IHttpRequest _request;
        private readonly ICurrentChain _currentChain;
        private readonly IRequestData _requestData;

        public RequestLogBuilder(IUrlRegistry urls, ISystemTime systemTime, IHttpRequest request, ICurrentChain currentChain, IRequestData requestData)
        {
            _urls = urls;
            _systemTime = systemTime;
            _request = request;
            _currentChain = currentChain;
            _requestData = requestData;
        }

        public RequestLog BuildForCurrentRequest()
        {
            var report = new ValueReport();
            try
            {
                _requestData.WriteReport(report);
            }
            catch (Exception)
            {
                // swallow the problem
            }

            var chainId = _currentChain.OriginatingChain == null ? Guid.Empty :
                _currentChain.OriginatingChain.UniqueId;
            var log = new RequestLog{
                ChainId    = chainId,
                Time = _systemTime.UtcNow(),
                RequestData = report,
                //ChainUrl = _urls.UrlFor(new ChainRequest{Id = chainId}),
                //DetailsUrl = _urls.UrlFor(new ChainDetailsRequest{Id = chainId})
            };

            if (_currentChain.OriginatingChain is RoutedChain)
            {
                log.HttpMethod = _request.HttpMethod();
                log.Endpoint = _request.RelativeUrl();
            }
            else if (_currentChain.OriginatingChain.InputType() != null)
            {
                log.Endpoint = _currentChain.OriginatingChain.InputType().FullName;
                log.HttpMethod = "n/a";
            }
            else
            {
                log.Endpoint = ChainVisualization.TitleForChain(_currentChain.OriginatingChain);
                log.HttpMethod = "n/a";
            }

            //log.ReportUrl = _urls.UrlFor(log);

            return log;
        }
    }
}