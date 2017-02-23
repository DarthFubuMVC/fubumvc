﻿using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Polling
{
    
    public class EnvelopeToken_is_polling_job_related_Tester
    {
        [Fact]
        public void is_not_related_to_a_job_with_no_message_at_all()
        {
            new EnvelopeToken().IsPollingJobRelated()
                .ShouldBeFalse();
        }

        [Fact]
        public void is_not_related_to_a_job_with_a_normal_message()
        {
            new EnvelopeToken{Message = new Message1()}
                .IsPollingJobRelated()
                .ShouldBeFalse();
        }

        [Fact]
        public void is_related_to_a_job_if_the_message_is_polling_job_request()
        {
            new EnvelopeToken
            {
                Message   = new JobRequest<APollingJob>()
            }.IsPollingJobRelated()
            .ShouldBeTrue();
        }
    }
}