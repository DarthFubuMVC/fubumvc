using System;
using System.Web;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using System.Web.Routing;
using Spark.Web.FubuMVC;
using Spark.Web.FubuMVC.ViewCreation;

namespace FubuMVC.HelloSpark
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            FubuApplication
				.For<HelloSparkRegistry>()
                .StructureMapObjectFactory()
				.Spark(spark =>
				       	{
				       		spark
								.Settings
								.AddViewFolder("/Features/");

							spark
								.Policies
								.Add<HelloSparkJavaScriptViewPolicy>()
								.Add<HelloSparkPolicy>();

				       		spark
				       			.Output
				       			.ToJavaScriptWhen(call => call.HasOutput && call.OutputType().Equals(typeof (JavaScriptResponse)));

				       	})
                .Bootstrap(RouteTable.Routes);
        }
    }
}