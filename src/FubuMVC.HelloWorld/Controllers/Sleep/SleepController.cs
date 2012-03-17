using System;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.Continuations;
using FubuMVC.HelloWorld.Controllers.Air;

namespace FubuMVC.HelloWorld.Controllers.Sleep
{
    public class SleepController
    {
        public Task<RemViewModel> RemAsync(RemInputModel remInput)
        {
            return Task<RemViewModel>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return new RemViewModel
                {
                    Message = "Url: {0} Slept for a second on Thread {1}".ToFormat(remInput.RawUrl, Thread.CurrentThread.ManagedThreadId)
                };
            }, TaskCreationOptions.AttachedToParent);
        }

        public Task InsomniaAsync(InsomniaInputModel input)
        {
            return Task.Factory.StartNew(() => Thread.Sleep(100),
                                         TaskCreationOptions.AttachedToParent);
        }

        public Task<FubuContinuation> BadDreamAsync()
        {
            return Task<FubuContinuation>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return FubuContinuation.RedirectTo<AirRequest>();
            }, TaskCreationOptions.AttachedToParent);
        }

        public Task<PartialSleepViewModel> PartialSleepAsync(PartialSleepInputModel sleepInput)
        {
            return Task<PartialSleepViewModel>.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                return new PartialSleepViewModel
                {
                    Message = "Slept for a while and returned partial model on Thread {0}".ToFormat(Thread.CurrentThread.ManagedThreadId)
                };
            }, TaskCreationOptions.AttachedToParent);
        }

        public Task HandleExceptionAsync(HandleExceptionAsyncInput input)
        {
            throw new Exception();
        }

        public Task DontHandleExceptionAsync(DontHandleExceptionAsyncInput input)
        {
            throw new DontHandleException();
        }
    }

    public class HandleExceptionAsyncInput
    {
    }

    public class DontHandleExceptionAsyncInput
    {
    }  

    public class RemViewModel
    {
        public string Message { get; set; }
    }

    public class RemInputModel
    {
        public string RawUrl { get; set; }
    }

    public class InsomniaInputModel
    {
        public string RawUrl { get; set; }
    }

    public class PartialSleepViewModel
    {
        public string Message { get; set; }
    }

    public class PartialSleepInputModel
    {
    }
}