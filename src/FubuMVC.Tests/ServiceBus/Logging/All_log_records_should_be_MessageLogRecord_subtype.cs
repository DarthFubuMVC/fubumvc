using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Monitoring;
using NUnit.Framework;

namespace FubuTransportation.Testing.Logging
{
    [TestFixture]
    public class All_log_records_should_be_MessageLogRecord_subtype
    {
        [Test]
        public void must_be_our_log_type()
        {
            var wrongTypes = typeof (MessageLogRecord).Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteTypeOf<LogRecord>() && !x.IsConcreteTypeOf<MessageLogRecord>())
                .Where(x => !x.Name.Contains("Polling") && 
                    !x.Name.Contains("ScheduledJob") && 
                    !x.CanBeCastTo<PersistentTaskMessage>() &&
                    x != typeof(ReceiveFailed))
                .ToList();

            if (wrongTypes.Any())
            {
                Assert.Fail("These types are not MessageLogRecord subtypes but should be: " + wrongTypes.Select(x => x.Name).Join(", "));
            }
        }
    }
}