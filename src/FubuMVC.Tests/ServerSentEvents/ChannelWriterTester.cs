using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServerSentEvents;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServerSentEvents
{
    [TestFixture]
    public class ChannelWriterTester
    {
        private IChannel<FakeTopic> theChannel;
        private ChannelWriter<FakeTopic> theChannelWriter;
        private RecordingServerEventWriter theWriter;
        private IServerEvent e1;
        private IServerEvent e2;
        private IServerEvent e3;
        private IServerEvent e4;
        private IServerEvent e5;
        private IServerEvent ie1;
        private IServerEvent ie2;
        private IServerEvent ie3;
        private FakeTopic theTopic;
        private IChannelInitializer<FakeTopic> theInitializer;
        private ITopicChannelCache theCache;

        [SetUp]
        public void SetUp()
        {
            theInitializer = new DefaultChannelInitializer<FakeTopic>();
            theWriter = new RecordingServerEventWriter();

            theCache = MockRepository.GenerateMock<ITopicChannelCache>();
            ITopicChannel<FakeTopic> channel = new TopicChannel<FakeTopic>(new EventQueue<FakeTopic>());
            theChannel = channel.Channel;
            theTopic = new FakeTopic();

            theCache.Stub(x => x.TryGetChannelFor(theTopic, out channel)).Return(true).OutRef(channel);

            theChannelWriter = new ChannelWriter<FakeTopic>(theWriter, theWriter, theCache, theInitializer);

            e1 = new ServerEvent("1", "data-1");
            e2 = new ServerEvent("2", "data-2");
            e3 = new ServerEvent("3", "data-3");
            e4 = new ServerEvent("4", "data-4");
            e5 = new ServerEvent("5", "data-5");

            ie1 = new ServerEvent("random1", "initialization data-1");
            ie2 = new ServerEvent("random2", "initialization data-2");
            ie3 = new ServerEvent("3", "initialization data-3");
        }

        [Test]
        public void simple_scenario_when_there_are_already_events_queued_up()
        {
            theChannel.Write(q => q.Write(e1, e2, e3));

            theWriter.ConnectedTest = () => !theWriter.Events.Contains(e3);

            var task = theChannelWriter.Write(theTopic);

            task.Wait(150).ShouldBeTrue();

            theWriter.Events.ShouldHaveTheSameElementsAs(e1, e2, e3);
        }

        [Test]
        public void sends_initialization_events_then_all_other_events()
        {
            theChannel.Write(q => q.Write(e1, e2, e3));
            theInitializer = new TestChannelInitializer(ie1, ie2);

            theChannelWriter = new ChannelWriter<FakeTopic>(theWriter, theWriter, theCache, theInitializer);

            theWriter.ConnectedTest = () => !theWriter.Events.Contains(e3);

            var task = theChannelWriter.Write(theTopic);

            task.Wait(150).ShouldBeTrue();

            theWriter.Events.ShouldHaveTheSameElementsAs(ie1, ie2, e1, e2, e3);
        }

        [Test]
        public void sends_initialization_events_then_events_following_last_initial_event_id()
        {
            theChannel.Write(q => q.Write(e1, e2, e3, e4, e5));
            theInitializer = new TestChannelInitializer(ie1, ie2, ie3);

            theChannelWriter = new ChannelWriter<FakeTopic>(theWriter, theWriter, theCache, theInitializer);

            theWriter.ConnectedTest = () => !theWriter.Events.Contains(e5);

            var task = theChannelWriter.Write(theTopic);

            task.Wait(150).ShouldBeTrue();

            theWriter.Events.ShouldHaveTheSameElementsAs(ie1, ie2, ie3, e4, e5);
        }

        [Test]
        public void polls_for_new_events()
        {
            theChannel.Write(q => q.Write(e1, e2));

            theWriter.ConnectedTest = () => !theWriter.Events.Contains(e5);

            var task = theChannelWriter.Write(theTopic);

            task.Wait(15);
            theChannel.Write(q => q.Write(e3, e4));
            task.Wait(15);
            theChannel.Write(q => q.Write(e5));

            task.Wait(150).ShouldBeTrue();

            theWriter.Events.ShouldHaveTheSameElementsAs(e1, e2, e3, e4, e5);
        }

        [Test]
        public void does_not_poll_when_the_client_is_initially_disconnected()
        {
            theWriter.ConnectedTest = () => false;

            var task = theChannelWriter.Write(theTopic);

            task.Wait(15).ShouldBeTrue();
        }

        [Test]
        public void does_not_write_poll_results_if_the_client_has_disconnected()
        {
            var task = theChannelWriter.Write(theTopic);

            task.Wait(150).ShouldBeFalse();

            theWriter.ForceClientDisconnect();

            theChannel.Write(q => q.Write(e1));

            task.Wait(150).ShouldBeTrue();

            theWriter.Events.ShouldHaveCount(0);
        }

        [Test]
        public void does_not_poll_when_the_channel_is_initially_disconnected()
        {
            theChannel.Flush();

            var task = theChannelWriter.Write(theTopic);

            task.Wait(15).ShouldBeTrue();
        }

        [Test]
        public void does_not_poll_when_the_channel_is_disconnected_after_initial_poll()
        {
            var task = theChannelWriter.Write(theTopic);

            task.Wait(500).ShouldBeFalse();

            theChannel.Flush();

            task.Wait(150).ShouldBeTrue();
            theWriter.Events.ShouldHaveCount(0);
        }

        [Test]
        public void sets_last_event_id_to_last_successfully_written_message()
        {
            theWriter.FailOnNthWrite = 3;

            theChannel.Write(q => q.Write(e1, e2, e3));

            theWriter.ConnectedTest = () => !theWriter.Events.Contains(e2);

            var task = theChannelWriter.Write(theTopic);

            task.Wait(15).ShouldBeTrue();

            theTopic.LastEventId.ShouldBe(e2.Id);
        }

        [Test]
        public void request_is_faulted_when_writer_throws_exception()
        {
            var requestCompletion = new RequestCompletion();
            AggregateException exception = null;
            var waitHandle = new ManualResetEvent(false);
            requestCompletion.WhenCompleteDo(ex =>
            {
                exception = ex as AggregateException;
                waitHandle.Set();
            });
            var asyncCoordinator = new AsyncCoordinator(requestCompletion, Enumerable.Empty<IExceptionHandler>());
            theWriter.WriterThrows = true;

            var task = theChannelWriter.Write(theTopic);
            theChannel.Write(q => q.Write(e1));

            asyncCoordinator.Push(task);

            waitHandle.WaitOne(TimeSpan.FromSeconds(1));
            var exceptions = exception.Flatten().InnerExceptions;

            exceptions.Count.ShouldBe(1);
            exceptions[0].Message.ShouldBe(RecordingServerEventWriter.ExceptionMessage);
        }

        [Test]
        public void parent_task_is_faulted_when_channel_initializer_fails()
        {
            theInitializer = new FailingTestChannelInitializer();
            theChannelWriter = new ChannelWriter<FakeTopic>(theWriter, theWriter, theCache, theInitializer);

            var testTask = new Task(() => theChannelWriter.Write(theTopic));

            testTask.RunSynchronously();
            testTask.IsFaulted.ShouldBeTrue();

            var exceptions = testTask.Exception.Flatten().InnerExceptions;

            exceptions.Count.ShouldBe(1);
            exceptions[0].Message.ShouldBe(FailingTestChannelInitializer.ExceptionMessage);
        }

        [Test]
        public void does_not_produce_stack_overflow_exception()
        {
            var parentTask = new Task(() => theChannelWriter.Write(theTopic));

            parentTask.Start();

            var startTime = DateTime.Now;
            while (DateTime.Now - startTime < TimeSpan.FromSeconds(15))
            {
                theChannel.Write(q => q.Write(e1));
                Thread.Sleep(2);
            }

            theWriter.ForceClientDisconnect();
            theChannel.Flush();

            parentTask.Wait(150).ShouldBeTrue();
        }

        [Test]
        public void failure_to_acquire_channel_terminates_without_errors()
        {
            theWriter = new RecordingServerEventWriter();

            var cache = MockRepository.GenerateMock<ITopicChannelCache>();
            ITopicChannel<FakeTopic> channel = new TopicChannel<FakeTopic>(new EventQueue<FakeTopic>());
            theChannel = channel.Channel;
            theTopic = new FakeTopic();

            cache.Stub(x => x.TryGetChannelFor(theTopic, out channel)).Return(false);

            theChannelWriter = new ChannelWriter<FakeTopic>(theWriter, theWriter, cache, theInitializer);
            var task = theChannelWriter.Write(theTopic);

            task.Wait(150).ShouldBeTrue();
        }
    }

    public class RecordingServerEventWriter : IServerEventWriter, IHttpRequest
    {
        private readonly IList<IServerEvent> _events = new List<IServerEvent>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public int? FailOnNthWrite { get; set; }

        public bool WriterThrows { get; set; }
        public const string ExceptionMessage = "Recording Server Event Writer Test Exception";

        public IList<IServerEvent> Events
        {
            get { return _lock.Read(() => _events.ToList()); }
        }

        public bool WriteData(object data, string id, string @event, int? retry)
        {
            throw new NotImplementedException();
        }

        public bool Write(IServerEvent @event)
        {
            if (WriterThrows)
                throw new Exception(ExceptionMessage);

            if (FailOnNthWrite.HasValue && FailOnNthWrite.Value == _lock.Read(() => _events.Count + 1))
                return false;

            _lock.Write(() => _events.Add(@event));
            return true;
        }

        public Func<bool> ConnectedTest = () => true;

        public IList<Action> QueuedActions = new List<Action>();

        private long _forceDisconnect;

        public bool IsClientConnected()
        {
            return ConnectedTest() && Interlocked.Read(ref _forceDisconnect) == 0;
        }

        public void ForceClientDisconnect()
        {
            Interlocked.Increment(ref _forceDisconnect);
        }


        string IHttpRequest.RawUrl()
        {
            throw new NotImplementedException();
        }

        string IHttpRequest.RelativeUrl()
        {
            throw new NotImplementedException();
        }

        string IHttpRequest.FullUrl()
        {
            throw new NotImplementedException();
        }

        string IHttpRequest.ToFullUrl(string url)
        {
            throw new NotImplementedException();
        }

        string IHttpRequest.HttpMethod()
        {
            throw new NotImplementedException();
        }

        bool IHttpRequest.HasHeader(string key)
        {
            throw new NotImplementedException();
        }

        IEnumerable<string> IHttpRequest.GetHeader(string key)
        {
            throw new NotImplementedException();
        }

        IEnumerable<string> IHttpRequest.AllHeaderKeys()
        {
            throw new NotImplementedException();
        }

        NameValueCollection IHttpRequest.QueryString
        {
            get { throw new NotImplementedException(); }
        }

        NameValueCollection IHttpRequest.Form
        {
            get { throw new NotImplementedException(); }
        }

        Stream IHttpRequest.Input
        {
            get { throw new NotImplementedException(); }
        }

        ICookies IHttpRequest.Cookies
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class TestChannelInitializer : IChannelInitializer<FakeTopic>
    {
        private readonly IServerEvent[] _initializationEvents;

        public TestChannelInitializer(params IServerEvent[] initializationEvents)
        {
            _initializationEvents = initializationEvents;
        }

        public IEnumerable<IServerEvent> GetInitializationEvents(FakeTopic topic)
        {
            var last = _initializationEvents.LastOrDefault();

            if (last != null)
                topic.LastEventId = last.Id;

            return _initializationEvents;
        }
    }

    public class FailingTestChannelInitializer : IChannelInitializer<FakeTopic>
    {
        public const string ExceptionMessage = "Intialization Exception";

        public IEnumerable<IServerEvent> GetInitializationEvents(FakeTopic Topic)
        {
            throw new Exception(ExceptionMessage);
        }
    }
}