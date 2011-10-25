using Bottles.Diagnostics;

namespace FubuMVC.Spark.SparkModel.Sharing
{
    public interface ISharingPolicy
    {
        void Apply(IPackageLog log, SharingGraph graph);
    }
}