using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core
{
    public interface IFubuRequestContext
    {
        IServiceLocator Services { get; }
        ICurrentHttpRequest Request { get; }
        IFubuRequest Models { get; }
        IOutputWriter Writer { get; }
    }

    public class FubuRequestContext : IFubuRequestContext
    {
        private readonly IServiceLocator _services;
        private readonly ICurrentHttpRequest _request;
        private readonly IFubuRequest _models;
        private readonly IOutputWriter _writer;

        public FubuRequestContext(IServiceLocator services, ICurrentHttpRequest request, IFubuRequest models, IOutputWriter writer)
        {
            _services = services;
            _request = request;
            _models = models;
            _writer = writer;
        }

        public IServiceLocator Services
        {
            get { return _services; }
        }

        public ICurrentHttpRequest Request
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
    }
}