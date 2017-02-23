﻿using FubuCore;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Scheduling;
using Xunit;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    
    public class ByTask_and_ByThread_SchedulerMakerTester
    {
        [Fact]
        public void builds_a_schedule_by_task()
        {
            var settings = new ThreadSettings
            {
                Count = 5
            };

            var channelNode = new ChannelNode();
            new ByTaskScheduleMaker<ThreadSettings>(x => x.Count)
                .As<ISettingsAware>()
                .ApplySettings(settings, channelNode);

            channelNode.Scheduler.ShouldBeOfType<TaskScheduler>()
                .TaskCount.ShouldBe(5);

        }

        [Fact]
        public void builds_a_schedule_by_threads()
        {
            var settings = new ThreadSettings
            {
                Count = 7
            };

            var channelNode = new ChannelNode();
            new ByThreadScheduleMaker<ThreadSettings>(x => x.Count)
                .As<ISettingsAware>()
                .ApplySettings(settings, channelNode);

            channelNode.Scheduler.ShouldBeOfType<ThreadScheduler>()
                .ThreadCount.ShouldBe(7);

        }


        public class ThreadSettings
        {
            public int Count { get; set; }
        }
    }
}