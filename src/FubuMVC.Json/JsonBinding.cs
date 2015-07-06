using FubuMVC.Core;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Json
{
    public class JsonBinding : IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            registry.Policies.Global.Add<ApplyJsonBindingPolicy>();

            registry.AlterSettings<ConnegSettings>(x => {
                var @default = x.FormatterFor(MimeType.Json);
                x.Formatters.Remove(@default);

                x.AddFormatter(new NewtonsoftJsonFormatter());
            });

            registry.Services<JsonServiceRegistry>();
        }
    }
}