using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Examples.HelloWorld.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Registration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Runtime;
using StructureMap;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace Examples.ServiceBus
{
    // SAMPLE: SimpleHandler
    public class SimpleHandler
    {
        public void Handle(PingMessage message)
        {
            Console.WriteLine("I got a ping!");
        }
    }
    // ENDSAMPLE



    // SAMPLE: AsyncHandler
    public interface IPongWriter
    {
        Task WritePong(PongMessage message);
    }

    public class AsyncHandler
    {
        private readonly IPongWriter _writer;

        public AsyncHandler(IPongWriter writer)
        {
            _writer = writer;
        }

        public Task Handle(PongMessage message)
        {
            return _writer.WritePong(message);
        }
    }
    // ENDSAMPLE

    // SAMPLE: Handlers-IMessage
    public interface IMessage { }

    public class MessageOne : IMessage { }
    // ENDSAMPLE

    // SAMPLE: Handlers-GenericMessageHandler
    public class GenericMessageHandler
    {
        private readonly Envelope _envelope;

        public GenericMessageHandler(Envelope envelope)
        {
            _envelope = envelope;
        }

        public void Consume(IMessage message)
        {
            Console.WriteLine($"Got a message from {_envelope.Source}");
        }
    }
    // ENDSAMPLE

    // SAMPLE: Handlers-SpecificMessageHandler
    public class SpecificMessageHandler
    {
        public void Consume(MessageOne message)
        {

        }
    }
    // ENDSAMPLE

    // SAMPLE: injecting-services-into-handlers
    public interface IMyService
    {

    }

    public class ServiceUsingHandler
    {
        private readonly IMyService _service;

        // Using constructor injection to get dependencies
        public ServiceUsingHandler(IMyService service)
        {
            _service = service;
        }

        public void Consume(PingMessage message)
        {
            // do stuff using IMyService with the PingMessage
            // input
        }
    }
    // ENDSAMPLE

    // SAMPLE: IHandler_of_T
    public interface IHandler<T>
    {
        void Handle(T message);
    }
    // ENDSAMPLE

    // SAMPLE: MyCustomHandlerSource
    public class MyCustomHandlerSource : IHandlerSource
    {
        public async Task<HandlerCall[]> FindCalls(Assembly applicationAssembly)
        {
            var types = await TypeRepository
                .FindTypes(applicationAssembly, TypeClassification.Concretes | TypeClassification.Closed);

            var candidates = types.Where(x => x.Closes(typeof(IHandler<>)));

            return candidates
                .SelectMany(findCallsPerType)
                .ToArray();
        }

        private static IEnumerable<HandlerCall> findCallsPerType(Type type)
        {
            return type
                .GetMethods()
                .Where(m => m.Name == nameof(IHandler<object>.Handle))
                .Select(m => new HandlerCall(type, m));
        }
    }
    // ENDSAMPLE

    // SAMPLE: CustomHandlerApp
    public class CustomHandlerApp : FubuTransportRegistry<AppSettings>
    {
        public CustomHandlerApp()
        {
            // Turn off the default handler conventions
            // altogether
            Handlers.DisableDefaultHandlerSource();

            // Custom handler finding through common options
            Handlers.FindBy(source =>
            {
                source.UseThisAssembly();
                source.IncludeClassesSuffixedWithHandler();
            });

            // Include candidate methods from a specific class or
            // classes
            Handlers.Include(typeof(SimpleHandler), typeof(AsyncHandler));

            // Include a specific handler class with a generic argument
            Handlers.Include<SimpleHandler>();

        }
    }
    // ENDSAMPLE

    // SAMPLE: AppWithCustomHandlerSources
    public class AppWithCustomHandlerSources : FubuTransportRegistry<AppSettings>
    {
        public AppWithCustomHandlerSources()
        {
            // Add a source by type
            Handlers.FindBy<MyHandlerSource>();

            // Add a source object directly
            Handlers.FindBy(new MyCustomHandlerSource());
        }
    }
    // ENDSAMPLE

    // SAMPLE: MyHandlerSource
    public class MyHandlerSource : HandlerSource
    {
        public MyHandlerSource()
        {
            // Search through other assemblies
            UseAssembly(typeof(PingMessage).GetTypeInfo().Assembly);

            // And the application assembly
            UseThisAssembly();


            // Use a custom filter to determine handler types
            IncludeTypes(type => type.Closes(typeof(IHandler<>)));

            // Or look for types with a known suffix
            IncludeClassesSuffixedWith("Handler");

            // Or use some built in filters
            IncludeClassesSuffixedWithConsumer();
        }
    }
    // ENDSAMPLE
}