using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using FubuMVC.Spark.SparkModel;
using FubuMVC.Spark.SparkModel.Scanning;
using FubuTestingSupport;
using NUnit.Framework;
using Spark;

namespace FubuMVC.Spark.Tests.SparkModel.Scanning
{
	[TestFixture]	
	public class SparkRequestExtensionsTester : InteractionContext<ScanRequest>
    {
		[Test]
	    public void include_spark_views_adds_correct_filter()
	    {
			ClassUnderTest.IncludeSparkViews();
			ClassUnderTest.Filters.ShouldContain("*{0}".ToFormat(Constants.DotSpark));
	    }
	}
}
