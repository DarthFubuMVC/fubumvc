using FubuMVC.Core;
using Spark.Web.FubuMVC.Registration;
using StructureMap;

namespace FubuMVC.HelloSpark
{
    public class Global : SparkStructureMapApplication
    {
        public override FubuRegistry GetMyRegistry()
        {
            return ObjectFactory.Container.GetInstance<HelloSparkRegistry>();
        }
    }
}