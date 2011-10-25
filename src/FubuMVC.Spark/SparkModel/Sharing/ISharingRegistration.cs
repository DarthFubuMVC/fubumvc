namespace FubuMVC.Spark.SparkModel.Sharing
{
    public interface ISharingRegistration
    {
        void Global(string global);
        void Dependency(string dependent, string dependency);
    }
}