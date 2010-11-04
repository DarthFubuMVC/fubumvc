using Spark.Web.FubuMVC.Registration;
using StructureMap;
using FubuMVC.Core;
using Spark.Web.FubuMVC;

namespace FubuMVC.HelloSpark
{
    public class Global : SparkStructureMapApplication
    {
        public override FubuRegistry GetMyRegistry()
        {
            return new HelloSparkRegistry(ObjectFactory.Container.GetInstance<SparkViewFactory>);
        }
    }
}