using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;
using FubuTransportation.Configuration;
using FubuTransportation.Runtime.Invocation;
using FubuTransportation.Subscriptions;

namespace FubuTransportation.Runtime
{

    public class TransportActivator : IActivator
    {
        private readonly ChannelGraph _graph;
        private readonly IServiceLocator _services;
        private readonly IHandlerPipeline _pipeline;
        private readonly IEnumerable<ITransport> _transports;
        private readonly IEnumerable<IFubuTransportActivator> _fubuTransportActivators;

        public TransportActivator(ChannelGraph graph, IServiceLocator services, IHandlerPipeline pipeline, IEnumerable<ITransport> transports, IEnumerable<IFubuTransportActivator> fubuTransportActivators)
        {
            _graph = graph;
            _services = services;
            _pipeline = pipeline;
            _transports = transports;
            _fubuTransportActivators = fubuTransportActivators;
        }

        public void Activate(IPackageLog log)
        {
            _graph.ReadSettings(_services);
            OpenChannels();
            _graph.StartReceiving(_pipeline);
            ExecuteActivators();
        }

        // virtual for testing
        public virtual void ExecuteActivators()
        {
            _fubuTransportActivators.Each(x => x.Activate());
        }

        // virtual for testing
        public virtual void OpenChannels()
        {
            try
            {
                _transports.Each(x => x.OpenChannels(_graph));

                var missingChannels = _graph.Where(x => x.Channel == null);
                if (missingChannels.Any())
                {
                    throw new InvalidOrMissingTransportException(_transports, missingChannels);
                }
            }
            catch (InvalidOrMissingTransportException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidOrMissingTransportException(e, _transports, _graph);
            }
        }
    }
}