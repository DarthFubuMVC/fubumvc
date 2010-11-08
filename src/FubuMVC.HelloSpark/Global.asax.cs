using Spark.Web.FubuMVC.Registration;
using StructureMap;
using Spark.Web.FubuMVC;

namespace FubuMVC.HelloSpark
{
    public class Global : SparkStructureMapApplication
    {
        public override SparkFubuRegistry GetMyRegistry()
        {
            return new HelloSparkRegistry(ObjectFactory.Container.GetInstance<SparkViewFactory>);
        }
    }
}