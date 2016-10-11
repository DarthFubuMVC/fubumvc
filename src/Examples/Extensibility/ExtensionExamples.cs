using System;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace Examples.Extensibility
{
    // SAMPLE: MarkingWithFubuModule
    [assembly: FubuModule]
    // ENDSAMPLE

    // SAMPLE: ActionLogger
    public class ActionLogger : WrappingBehavior
    {
        private readonly ICurrentChain _chain;
        private readonly ILogger _logger;

        public ActionLogger(ICurrentChain chain, ILogger logger)
        {
            _chain = chain;
            _logger = logger;
        }

        protected override void invoke(Action action)
        {
            _logger.Debug($"Running chain {_chain.Current.Title()}");
            action();
        }
    }
    // ENDSAMPLE

    // SAMPLE: MyLoggingPolicy
    public class MyLoggingPolicy : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            foreach (var chain in graph.Chains)
            {
                chain.WrapWith<ActionLogger>();
            }
        }
    }
    // ENDSAMPLE

    // SAMPLE: FubuAppWithLoggingPolicy
    public class FubuAppWithLoggingPolicy : FubuRegistry
    {
        public FubuAppWithLoggingPolicy()
        {
            // Apply the MyLoggingPolicy to every
            // chain in the system
            Policies.Global.Add<MyLoggingPolicy>();

            // or

            // Apply the MyLoggingPolicy to only
            // the chains discovered from the main application
            // or a local extension
            Policies.Local.Add<MyLoggingPolicy>();
        }
    }
    // ENDSAMPLE

    public class MySpecialLogger : ILogger
    {
        public void Debug(string message, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Info(string message, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void Error(string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Error(object correlationId, string message, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void Debug(Func<string> message)
        {
            throw new NotImplementedException();
        }

        public void Info(Func<string> message)
        {
            throw new NotImplementedException();
        }

        public void DebugMessage(LogTopic message)
        {
            throw new NotImplementedException();
        }

        public void InfoMessage(LogTopic message)
        {
            throw new NotImplementedException();
        }

        public void DebugMessage<T>(Func<T> message) where T : class, LogTopic
        {
            throw new NotImplementedException();
        }

        public void InfoMessage<T>(Func<T> message) where T : class, LogTopic
        {
            throw new NotImplementedException();
        }
    }

    // SAMPLE: LoggingExtension
    public class LoggingExtension : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Global.Add<MyLoggingPolicy>();

            registry.Services.ReplaceService<ILogger, MySpecialLogger>();
        }
    }
    // ENDSAMPLE

    // SAMPLE: ImportLoggingExtension
    public class MyFubuApp : FubuRegistry
    {
        public MyFubuApp()
        {
            // Explicitly apply LoggingExtension
            Import<LoggingExtension>();
        }
    }
    // ENDSAMPLE

    // SAMPLE: MyExtensionRegistry
    public class MyExtensionRegistry : FubuPackageRegistry
    {
        public MyExtensionRegistry() : base("extension")
        {
            // Will locate any handlers or endpoints from this
            // Assembly

            // Apply the MyLoggingPolicy to only the
            // chains from this extension
            Policies.Local.Add<MyLoggingPolicy>();

            // Can override services
            Services.ReplaceService<ILogger, MySpecialLogger>();

            // Can alter settings to the main app too
            AlterSettings<AssetSettings>(_ =>
            {
                _.AllowableExtensions.Add(".xls");
            });
        }
    }
    // ENDSAMPLE


}