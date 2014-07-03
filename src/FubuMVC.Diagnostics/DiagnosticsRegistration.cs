using FubuCore.Binding.InMemory;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Diagnostics
{
    public class DiagnosticsRegistration : IFubuRegistryExtension
    {
        public const string DIAGNOSTICS_URL_ROOT = "_fubu";

        public void Configure(FubuRegistry registry)
        {
            MimeType.New("text/jsx", ".jsx");

            registry.Services<DiagnosticServiceRegistry>();

            registry.AlterSettings<AssetSettings>(x => x.AllowableExtensions.Add(".jsx"));

            registry.AlterSettings<DiagnosticsSettings>(x =>
            {
                x.TraceLevel = TraceLevel.Verbose;
            });

            registry.Configure(graph => {
                var settings = graph.Settings.Get<DiagnosticsSettings>();

                if (settings.TraceLevel == TraceLevel.Verbose)
                {
                    graph.Services.Clear(typeof (IBindingLogger));
                    graph.Services.AddService<IBindingLogger, RecordingBindingLogger>();

                    graph.Services.Clear(typeof (IBindingHistory));
                    graph.Services.AddService<IBindingHistory, BindingHistory>();

                    graph.Services.AddService<ILogListener, RequestTraceListener>();
                }

                if (settings.TraceLevel == TraceLevel.Production)
                {
                    graph.Services.AddService<ILogListener, ProductionModeTraceListener>();
                }
            });
        }
    }
}