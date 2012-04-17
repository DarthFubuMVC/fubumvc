.. _urlconventions:

==============================
Out-of-the-Box URL Conventions
==============================

As is the case with everything in FubuMVC, the goal is to decide up-front what
configuration makes the most sense for your web application. After such
conventions are setup, then the framework will get out of your way.
Configuration & other infrastructure concerns will not bleed into your
application's core business logic. In FubuMVC, there is no need to maintain a
messy list of regular expressions in a ``Routes.cs`` file, as is the case with
ASP.NET MVC.  FubuMVC comes with sensible default conventions you can
immediately utilize, or you have the power to craft your own URL policies
exactly the way you want.

URLs from Method Names
----------------------

The first pattern checked by the default url conventions is the
``MethodToUrlBuilder``. It is activated when a controller action method name
contains any number of underscore characters. It allows you to specify the HTTP
verb and the entire route from a single method name. Here is an example

.. code-block:: csharp

   public class TestController
   {
       public string post_Data()
       {
           return "POST only: /data";
       }

       public string get_SomeTest_Route()
       {
           return "GET only: /sometest/route";
       }

       public string Any_Http_Verb()
       {
           return "Any HTTP Verb: /any/http/verb";
       }
   }

The first controller action will map to the ``/data`` route and be constrained
to the ``POST`` HTTP verb. The second action will map to the ``/sometest/route``
URL and be constrained to only ``GET`` requests. The last action method will
accept all requests with any HTTP verb on the route ``/any/http/verb``. You may
have also noticed that underscore characters in the method names become slashes
in the resultant URL.

You can also pair this strategy with Model Binding

