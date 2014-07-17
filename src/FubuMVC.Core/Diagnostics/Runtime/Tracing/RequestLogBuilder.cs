using System;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Dates;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Diagnostics.Runtime.Tracing
{
    public class RequestLogBuilder : IRequestLogBuilder
    {
        private readonly ISystemTime _systemTime;
        private readonly IHttpRequest _request;
        private readonly ICurrentChain _currentChain;
        private readonly IRequestData _requestData;
        private readonly DiagnosticsSettings _settings;

        public RequestLogBuilder(ISystemTime systemTime, IHttpRequest request, ICurrentChain currentChain, IRequestData requestData, DiagnosticsSettings settings)
        {
            _systemTime = systemTime;
            _request = request;
            _currentChain = currentChain;
            _requestData = requestData;
            _settings = settings;
        }

        public RequestLog BuildForCurrentRequest()
        {
            var log = new RequestLog{
                Hash    = _currentChain.OriginatingChain.GetHashCode(),
                Time = _systemTime.UtcNow()
            };

            if (_settings.TraceLevel == TraceLevel.Verbose)
            {
                // Get the request headers here
            }

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
                log.Endpoint =_currentChain.OriginatingChain.Title();
                log.HttpMethod = "n/a";
            }

            return log;
        }
    }
}