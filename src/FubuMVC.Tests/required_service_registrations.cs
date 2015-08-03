using System;
using System.Diagnostics;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Binding.InMemory;
using FubuCore.Conversion;
using FubuCore.Formatting;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Continuations;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.Assets;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Json;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Resources.Conneg;
using FubuMVC.Core.Resources.PathBased;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Aggregation;
using FubuMVC.Core.Runtime.Conditionals;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Runtime.SessionState;
using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;
using FubuMVC.Core.Urls;
using FubuMVC.Core.View;
using NUnit.Framework;
using StructureMap;

namespace FubuMVC.Tests
{
    [TestFixture]
    public class required_service_registrations
    {
        [Test]
        public void all_default_service_registrations()
        {
            using (var runtime = FubuRuntime.Basic())
            {
                var _ = runtime.Get<IContainer>();


                Debug.WriteLine(_.WhatDoIHave());

                // Model Binding registrations
                _.DefaultRegistrationIs<IBindingContext, BindingContext>();
                _.DefaultRegistrationIs<IObjectResolver, ObjectResolver>();
                _.DefaultRegistrationIs<IBindingLogger, NulloBindingLogger>();
                _.ShouldHaveRegistration<IModelBinder, ResourcePathBinder>();
                _.ShouldHaveRegistration<IConverterFamily, AspNetPassthroughConverter>();


                // Core services
                _.DefaultRegistrationIs<IAggregator, Aggregator>();
                _.DefaultRegistrationIs<IFubuRequestContext, FubuRequestContext>();
                _.DefaultRegistrationIs<IRequestData, FubuMvcRequestData>();
                _.DefaultRegistrationIs<IAsyncCoordinator, AsyncCoordinator>();
                _.DefaultRegistrationIs<IExceptionHandlingObserver, ExceptionHandlingObserver>();
                _.DefaultRegistrationIs<ICookies, Cookies>();
                _.DefaultRegistrationIs<IFlash, FlashProvider>();
                _.DefaultRegistrationIs<IOutputWriter, OutputWriter>();
                _.DefaultRegistrationIs<IPartialFactory, PartialFactory>();
                _.DefaultRegistrationIs<IPartialInvoker, PartialInvoker>();
                _.DefaultRegistrationIs<IRequestDataProvider, RequestDataProvider>();
                _.DefaultRegistrationIs<Stringifier, Stringifier>(); // it's goofy, but assert that it exists

                _.ShouldHaveRegistration<IActivator, DisplayConversionRegistryActivator>();

                _.DefaultSingletonIs<IChainResolver, ChainResolutionCache>();
                _.DefaultSingletonIs<IClientMessageCache, ClientMessageCache>();

                _.DefaultRegistrationIs<IDisplayFormatter, DisplayFormatter>();
                _.DefaultRegistrationIs<IEndpointService, EndpointService>();
                _.DefaultRegistrationIs<IFileSystem, FileSystem>();

                _.ShouldHaveRegistration<ILogModifier, LogRecordModifier>();

                _.DefaultRegistrationIs<IObjectConverter, ObjectConverter>();
                _.DefaultRegistrationIs<ISetterBinder, SetterBinder>();

                _.DefaultRegistrationIs<IFubuApplicationFiles, FubuApplicationFiles>();

                _.DefaultRegistrationIs<IUrlRegistry, UrlRegistry>();
                _.DefaultRegistrationIs<IChainUrlResolver, ChainUrlResolver>();
                _.DefaultRegistrationIs<AppReloaded, AppReloaded>();

                _.DefaultRegistrationIs<ILogger, Logger>();

                // Conneg
                _.DefaultRegistrationIs<IResourceNotFoundHandler, DefaultResourceNotFoundHandler>();



                // Conditionals
                _.DefaultRegistrationIs<IContinuationProcessor, ContinuationProcessor>();
                _.DefaultRegistrationIs<IConditionalService, ConditionalService>();

                // Assets
                _.DefaultRegistrationIs<IAssetTagBuilder, AssetTagBuilder>();
                _.DefaultSingletonIs<IAssetFinder, AssetFinderCache>();

                // Diagnostics
                _.DefaultSingletonIs<IDiagnosticAssets, DiagnosticAssetsCache>();

                // Security
                _.DefaultRegistrationIs<IAuthenticationContext, WebAuthenticationContext>();
                _.DefaultRegistrationIs<ISecurityContext, WebSecurityContext>();
                _.DefaultRegistrationIs<IAuthorizationPreviewService, AuthorizationPreviewService>();
                _.DefaultRegistrationIs<IChainAuthorizor, ChainAuthorizor>();
                _.DefaultRegistrationIs<IAuthorizationFailureHandler, DefaultAuthorizationFailureHandler>();


                // Json
                _.DefaultRegistrationIs<IJsonSerializer, NewtonSoftJsonSerializer>();
            }
        }

        [Test]
        public void IAssetTagBuilder_is_registered_in_development_mode()
        {
            using (var runtime = FubuRuntime.Basic(_ => _.Mode = "development"))
            {
                runtime.Get<IContainer>().DefaultRegistrationIs<IAssetTagBuilder, DevelopmentModeAssetTagBuilder>();
            }
        }
    }
}