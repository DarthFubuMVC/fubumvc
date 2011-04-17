using FubuMVC.Core;
using FubuMVC.Spark.Tokenization;

namespace FubuMVC.Spark
{
    public class SparkRegistry : FubuRegistry, IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            var tokenizer = new ViewTokenizer()
                .AddEnricher<MasterPageEnricher>()
                .AddEnricher<ViewModelEnricher>()
                .AddEnricher<NamespaceEnricher>();

            registry.Views.Facility(new SparkViewFacility(tokenizer));
        }
    }
}