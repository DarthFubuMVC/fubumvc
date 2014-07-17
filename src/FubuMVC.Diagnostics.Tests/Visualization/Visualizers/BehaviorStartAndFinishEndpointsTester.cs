using System;
using FubuMVC.Core.Diagnostics.Runtime;
using FubuMVC.Core.Diagnostics.Runtime.Tracing;
using FubuMVC.Diagnostics.Visualization.Visualizers;
using FubuMVC.Tests.Registration;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace FubuMVC.Diagnostics.Tests.Visualization.Visualizers
{
    [TestFixture]
    public class BehaviorStartAndFinishEndpointsTester
    {
        [Test]
        public void visualize_BehaviorFinish_with_no_exceptions()
        {
            var finish = new BehaviorFinish(new BehaviorCorrelation(new FakeNode()));

            var tag = new BehaviorStartAndFinishEndpoints().VisualizePartial(finish);

            tag.Children.Any(x => x.HasClass("exception")).ShouldBeFalse();
            tag.Next.ShouldBeNull();
        }

        [Test]
        public void visualize_BehaviorFinish_with_an_exception()
        {
            var finish = new BehaviorFinish(new BehaviorCorrelation(new FakeNode()));
            finish.LogException(new NotImplementedException());

            var tag = new BehaviorStartAndFinishEndpoints().VisualizePartial(finish);
            tag.Children.FirstOrDefault(x => x.HasClass("exception")).Text().ShouldEqual(
                typeof (NotImplementedException).Name);

            tag.Next.ShouldBeOfType<ExceptionReportTag>();
        
        }
    }
}