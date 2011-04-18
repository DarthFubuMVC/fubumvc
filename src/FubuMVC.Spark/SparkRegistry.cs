using FubuMVC.Core;
using FubuMVC.Spark.Tokenization;

namespace FubuMVC.Spark
{
    public class SparkRegistry : FubuRegistry, IFubuRegistryExtension
    {
        public void Configure(FubuRegistry registry)
        {
            var tokenizer = new ViewTokenizer()
                .AddModifier<MasterPageModifier>()
                .AddModifier<ViewModelModifier>()
                .AddModifier<NamespaceModifier>();

            registry.Views.Facility(new SparkViewFacility(tokenizer));
        }
    }
}