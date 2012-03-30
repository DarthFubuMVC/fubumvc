========
Overview
========

This guide covers getting up and running a simple FubuMVC application.
After reading it, you should be familiar with:

    * What FubuMVC is, why I might (or might not) use it, and how I get started

    * Using NuGet to download and install FubuMVC in a new Visual Studio.NET
      project

    * The general layout of a FubuMVC project

    * The basic architectural principles of FubuMVC

    * Adding simple HTTP endpoints in FubuMVC

.. note::

    All the code used in this guide is available under the QuickStart project
    in the `FubuMVC repository on GitHub
    <http://github.com/DarthFubuMVC/fubumvc>`_

This Guide Assumes...
=====================

    *  That when you approach a new web application project, your first thought
       isn't to open the designer and start dragging-and-dropping your way to
       the finish line.

    *  That you have a copy of Visual Studio 2010 (any SKU). While it is
       possible to run FubuMVC on Mono/Linux with nginx, Apache, or XSP, this
       guide assumes that you are working with Visual Studio.NET.

    *  NuGet is installed on your machine

    *  You are familiar with building basic ASP.NET applications (either
       WebForms or MVC) and know how to basically get an ASP.NET app working in
       IIS7 (Windows 2008, Windows 7).

    *  That you have a good working knowledge of C# 3.0 and .NET 3.5 in general
       including generics, lambda expressions, and using interfaces.

    *  That you have a desire to build .NET/ASP.NET-based web applications and
       are looking for a web framework that doesn't get in your way as much,
       allows you to test as much as possible, and helps you stay focused on
       your code and not pleasing your web framework.

.. note::

    FubuMVC can work with IIS 6 (Windows 2003) and IIS 6.1 (XP), but it's more
    complicated to set up. This guide will focus on IIS 7.

Introduction to FubuMVC
=======================

What is FubuMVC?
----------------

FubuMVC is a `Front Controller-style
<http://martinfowler.com/eaaCatalog/frontController.html>`_ MVC framework for
.NET written in the C# language. FubuMVC was started by a team of developers
who wanted to stay on the .NET platform but were unhappy with the shape and
direction of ASP.NET MVC. The goals and architectural philosophy of FubuMVC is:

    * Exploit the idea of `convention over configuration
      <http://msdn.microsoft.com/en-us/magazine/dd419655.aspx>`_ for better
      productivity, but allow users to teach FubuMVC about their own
      conventions against a well-defined semantic model.

    * Decouple your application code from framework code and keep your
      application relatively free from the scourges of XML configuration,
      inheritance tangles, and attribute soup.

    * Remove friction from automated testing of your application code.

    * Maximize the ability to use composition throughout your application by
      focusing on SOLID principles, separation of concerns, `DRY
      <http://en.wikipedia.org/wiki/Don't_repeat_yourself>`_. Internally,
      FubuMVC uses your application's Inversion of Control container to build
      out its own dependencies.

    * Modular architectures. FubuMVC includes the most sophisticated and
      capable infrastructure for modularizing and extending web applications in
      the .NET ecosystem.

    * Provide a less painful development experience with informative
      diagnostics about your application

    * Use static typing in an advantageous way. Many other alternative web
      frameworks in the .NET space are faithful copies of Ruby or Python
      frameworks that have neither the strengths of Ruby/Python or C#. FubuMVC
      uses the rich metadata from static typing to drive conventions and reduce
      errors. If you prefer Ruby on Rails or Sinatra style development, we
      think you should use Ruby on Rails or Sinatra instead of attempting to
      work that way inside of C#.

    * Eliminate the dreaded "tag soup" problem in views by using advanced HTML
      helpers and conventions to DRY up your views

What do I need to know before I get started?
--------------------------------------------

FubuMVC is open source. That means it's developed, for free, by dedicated
individuals without a lot of free time (we have families and day jobs). While
we try to provide a nice, polished experience for developer, it can't always
succeed in that objective. FubuMVC is licensed under the permissive Apache 2.0
license: `https://github.com/DarthFubuMVC/fubumvc/raw/master/license.txt
<https://github.com/DarthFubuMVC/fubumvc/raw/master/license.txt>`_.
Contributions are most certainly welcome, just fork the `Git repository
<https://github.com/DarthFubuMVC/fubumvc>`_ and start firing off pull requests.

As of May 2011, FubuMVC is powering production websites at 56 companies with
more in development. While we know that FubuMVC will never attain the usage of
the official Microsoft offerings, we would like to have more users just to make
the ecosystem richer and more viable for the long run.

.. note::

    FubuMVC will be rougher and more raw than something you might get from
    Microsoft or a 3rd party library vendor. Please bear with us as we improve
    and make FubuMVC better. Your feedback and suggestions are always welcome.
    Your assistance and time are welcome more, though :)

Core Architecture
=================

FubuMVC takes a slightly different approach to the classic `Model 2 Model View
Controller pattern <http://en.wikipedia.org/wiki/Model_2>`_ that puts more
focus on composing a pipeline of what we call "behaviors" that are completely
unaware of each other rather than the traditional Model-View-Controller triad.

A typical web request for a view will look something like:

    #. An HTTP request is received by the web server. The ASP.NET routing
       module acts as a front controller to choose a "chain" of behaviors to
       execute.

    #. The first behavior calls an "action" (think Controller) that interacts
       with the rest of your application and returns a view model representing
       the data to be rendered by the view.

    #. A second behavior invokes a WebForms or Spark view to render the view
       model that was returned from the action in the previous step.

As far as a developer is concerned, all the familiar elements of classic MVC
are present, but there's nothing to stop you from composing a different
pipeline of behaviors for more sophisticated request handling.

View Models
-----------

View models are typically `Data Transfer Objects
<http://en.wikipedia.org/wiki/Data_transfer_object>`_ that are either inputs to
or outputs from behaviors. It's probably advantageous to think of view models
as messages passed to or between behaviors. As a baked in "opinion," FubuMVC
strongly prefers that the input models are unique across actions in your
application. FubuMVC can use an input model to resolve URL's or authorization
rules in the system. Likewise, output model signatures from action methods are
used to "attach" view and other output behaviors to a behavior chain.

Actions (Controllers)
---------------------

In classic MVC the controllers have the responsibility of processing the HTTP
input and deciding what data was to be displayed by the view layer.  In FubuMVC
this responsibility is performed by what we simply call "actions." Actions in
FubuMVC are just methods on concrete `POCO
<http://en.wikipedia.org/wiki/Plain_Old_CLR_Object>`_ classes in your
application that will be called during an HTTP request like the following:

.. literalinclude:: ../../../src/FubuMVC.GuideApp/Controllers/Home/Home.cs
   :lines: 5-20
   :linenos:

Typically, you will let FubuMVC marshal the raw HTTP data into an input model,
then FubuMVC will call your action method directly and store any output where
later behaviors can find it. This is what we call the "one model in, one model
out" philosophy, meaning that actions should typically only be concerned with
your application's data and services rather than exercising framework
machinery.

One of the original goals of FubuMVC was to simplify our controller actions so
that all they had to do was process a request object and return a response with
no coupling to giant base classes or repetitive boilerplate code just to feed
the framework. We believe that the "one model in, one model out" opinion
makes our code easier to read, write, and test by removing the noise code so
prevalent in other .NET solutions. It also greatly improves our ability to
compose the runtime pipeline and creates traceability between parts of the
application.

Views
-----

Now that you've got view models and actions to process them, you need something
to render the view model into HTML. As of this writing, FubuMVC supports the
Web Forms and Spark view engines. In addition, you can happily have actions
spit out raw HTML or `HtmlTag/HtmlDocument
<https://github.com/DarthFubuMVC/htmltags>`_ objects.

Behaviors and Behavior Chains
-----------------------------

During day to day development most developers are going to be working strictly
with view models, actions, and views. Internally, the FubuMVC framework sees
all these things as a chain of "behavior" objects nested within each other in
what we frequently refer to as the `Russian Doll Model
<http://codebetter.com/jeremymiller/2011/01/09/fubumvcs-internal-runtime-the-russian-doll-model-and-how-it-compares-to-asp-net-mvc-and-openrasta/>`_.

Even a simple HTTP request is likely to be handled by multiple behaviors. While
FubuMVC comes out of the box with behaviors for common tasks like Json
serialization/deserialization, calling actions, and rendering views, you can
build your own custom behaviors.

Wrappers
--------

Wrappers are simply behaviors that you can use to perform additional work
during an HTTP request like authorization checks, caching, activity tracking,
or just extra auditing.

BehaviorGraph and FubuRegistry
------------------------------

FubuMVC contains a configuration model called BehaviorGraph that completely
models how each possible HTTP endpoint will be handled. For each HTTP endpoint
in the system, BehaviorGraph models:

  #. Routes and URL's 

  #. Behavior Chains

  #. Actions to be called

  #. Views or other output behaviors like Json output that will be called

  #. Authorization rules

  #. Wrappers

You won't work directly with these objects daily, but understanding the
underlying BehaviorGraph model is crucial to writing your own FubuMVC
conventions and policies later.

Inversion of Control Container
------------------------------

FubuMVC is built around the idea of composition, but that composition can come
at a cost. FubuMVC uses your application’s IoC container to assemble all the
various pieces, services, behaviors, and extensions that make up a functioning
FubuMVC application. We like to say that FubuMVC is "Dependency Injection
turtles all the way down," meaning that **all** FubuMVC services are resolved
from the IoC container without hacks like "IDependencyResolver."

.. note::

    At this writing (May 2011), FubuMVC only supports the StructureMap container,
    but work is ongoing to add Castle Windsor support for a following release.

Bottles
-------

FubuMVC uses the `Bottles project <https://github.com/DarthFubuMVC/bottles>`_
for modularity.  Bottles can be used to break your application up into
different "areas" or to extend your application with new content or abilities.

Web.config dependencies
-----------------------

I wish it wasn't so, but for now FubuMVC has some required dependencies that
must be configured via XML in web.config:

    #. The System.Web.Routing.UrlRoutingModule module must be registered

    #. Access to the folder "Content" should be authorized for all users
       (this is where FubuMVC assumes that content like images, scripts, and
       styles are stored)

    #. Access to the folder "\_content" should be authorized for all users
       (this is where FubuMVC assumes that content like images, scripts, and
       styles are stored for packages. This will be changed in the near
       term)

    #. Access to the folder "fubu-content" should be denied for all users.
       This folder is related to the Bottles support in FubuMVC

