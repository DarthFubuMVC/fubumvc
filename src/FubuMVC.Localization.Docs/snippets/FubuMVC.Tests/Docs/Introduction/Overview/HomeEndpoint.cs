using System;

namespace FubuMVC.Tests.Docs.Introduction.Overview
{
    // SAMPLE: overview-homecontroller
    // The suffix "Endpoint" is a default
    // convention of FubuMVC.  
    public class HomeEndpoint
    {
        // By returning 
        public HomeViewModel Index()
        {
            return new HomeViewModel();
        }

        public string get_hello()
        {
            return "Hello!";
        }

        public Response post_update(Request request)
        {
            return new Response();
        }
    }

    public class Request{}
    public class Response{}

    public class HomeViewModel
    {
    }
    // ENDSAMPLE

    // SAMPLE: models
    public class InputModel
    {
        public Guid Id { get; set; }
    }

    public class OutputModel
    {
        public string Statue { get; set; }
    }

    // 'ViewModel' just means an object that is
    // consumed by a matching Spark or Razor view
    public class ViewModel
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    // ENDSAMPLE
}