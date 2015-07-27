using System;
using System.Threading.Tasks;
using FubuMVC.Core;

namespace FubuMVC.Tests.ServiceBus.Docs.Basics
{
    // SAMPLE: CustomHandlerNamingSample
    public class CustomHandlerTransportRegistry : FubuRegistry
    {
        public CustomHandlerTransportRegistry()
        {
            Handlers.FindBy(x =>
            {
                //Define your own conventions here
            });
        }
    }
    // ENDSAMPLE

    // SAMPLE: ReceivingSampleHandler
    public class ReceivingSampleHandler
    {
        public void Handle(MyMessage message)
        {
            //do something
        }

        public async Task HandleAsync(MyMessage message)
        {
            await Task.Delay(100);
            //do something
        }

        public MyResponse NameDoesntMatter(MyMessage message)
        {
            return new MyResponse();
        }

        public async Task<MyResponse> HandleWithResponseAsync(MyMessage message)
        {
            await Task.Delay(100);
            return new MyResponse();
        }
    }
    // ENDSAMPLE

    public class MyMessage
    {
    }

    public class MyResponse
    {
    }

    // SAMPLE: ReceivingPolymorphismSample
    public class LoginSucceededHandler
    {
        public void Handle(LoginSucceeded message)
        {
            //record success
        }
    }

    public class LoginSuccededV2Handler
    {
        public void Handle(LoginSucceededV2 message)
        {
            //record time
        }
    }

    public class LoginSucceeded
    {
        public string Username { get; set; }
    }

    public class LoginSucceededV2 : LoginSucceeded
    {
        public DateTime SucceededAt { get; set; }
    }
    // ENDSAMPLE
}