using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    public interface IFubuRequestContext
    {
        IServiceLocator Services { get; }
        IHttpRequest Request { get; }
        IFubuRequest Models { get; }
        IOutputWriter Writer { get; }
        ILogger Logger { get; }
    }

    public static class FubuRequestContextExtensions
    {
        public static T Service<T>(this IFubuRequestContext context)
        {
            return context.Services.GetInstance<T>();
        }
    }

    public class FubuRequestContext : IFubuRequestContext
    {
        private readonly IServiceLocator _services;
        private readonly IHttpRequest _request;
        private readonly IFubuRequest _models;
        private readonly IOutputWriter _writer;
        private readonly ILogger _logger;

        public FubuRequestContext(IServiceLocator services, IHttpRequest request, IFubuRequest models, IOutputWriter writer, ILogger logger)
        {
            _services = services;
            _request = request;
            _models = models;
            _writer = writer;
            _logger = logger;
        }

        public IServiceLocator Services
        {
            get { return _services; }
        }

        public IHttpRequest Request
        {
            get { return _request; }
        }

        public IFubuRequest Models
        {
            get { return _models; }
        }

        public IOutputWriter Writer
        {
            get { return _writer; }
        }

        public ILogger Logger
        {
            get { return _logger; }
        }
    }

}