using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.View.Attachment;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration.Conventions 
{
	[TestFixture]
	public class ApplyingACustomViewBagConvention
	{
		readonly CustomViewBagConvention _convention = new CustomViewBagConvention();
		[SetUp]
		public void Setup ()
		{
			new FubuRegistry(x => x.Views.ApplyConvention(_convention)).BuildGraph();
		}

		[Test]
		public void can_apply_custom_viewbagconvention()
		{
			_convention.Invocations.ShouldEqual(1);
		}

		public class CustomViewBagConvention : IViewBagConvention 
		{
			public int Invocations { get; private set; }
			public void Configure(ViewBag bag, BehaviorGraph graph)
			{
				Invocations++;
			}
		}
	}
}
