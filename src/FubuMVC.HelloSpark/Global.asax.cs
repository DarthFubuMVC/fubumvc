using Spark.Web.FubuMVC.Bootstrap;
using StructureMap;
using FubuMVC.Core;
using Spark.Web.FubuMVC;

namespace FubuMVC.HelloSpark
{
    public class Global : SparkStructureMapApplication
    {
        public override FubuRegistry GetMyRegistry()
        {
            var sparkViewFactory = ObjectFactory.Container.GetInstance<SparkViewFactory>();
            return new HelloSparkRegistry(EnableDiagnostics, ControllerAssembly, sparkViewFactory);
        }
    }
}