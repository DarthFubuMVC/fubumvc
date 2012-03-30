===============
Getting Started
===============

Hello, FubuMVC!
===============

Let's do the inevitable "Hello, World!" exercise in FubuMVC.

Setup a new FubuMVC application
-------------------------------

.. image:: images/getting_started/SelectNuget.png
   :alt: Installing FubuMVC via NuGet

In Solution Explorer, your project layout should look like this:

.. image:: images/getting_started/ProjectLayout1.png
   :alt: Adding the NuGet reference adds a few pieces to your application

Adding the NuGet reference adds a few pieces to your application:

    #. Some directives to the web.config file.

    #. A basic FubuRegistry class called "ConfigureFubuMVC" that registers
       some basic conventions for your application.

    #. Using the WebActivator library, adds a call in the application start
       event to the FubuApplication class to bootstrap the FubuMVC model,
       Route's, and your IoC container.

    #. The FubuMVC.GettingStarted package in /fubu-content (you'll remove
       this later).

Fire up your browser!
---------------------

Now, just for fun, let's fire up the application and see what happens.  Make
sure your new QuickStart project is the startup project for your solution and
press ``F5`` to start debugging. Your browser should open to this page:

.. figure:: images/getting_started/GettingStartedPage.png

Don't worry about where that content came from, we'll get to that later - and
how to get rid of it too. Before we get into the details of just what it is
that NuGet dumped into your application, let's do the obligatory "Hello,
world!" exercise.

Say "Hello!"
------------

Alright, assuming that we've got a working application, let's add our first
endpoint to the system. Add a class to your project like this to your project:

.. literalinclude:: ../../../src/QuickStart/SayHelloController.cs
   :lines: 1-11
   :linenos:

Compile and restart the web application to bring up the home page again, but
this time, go to the URL for ~/helloworld. You should see this:

.. image:: images/getting_started/HelloWorld.png

Pretty cool, right? You managed to spit out a string to the browser window.
Let's try it again, but this time, let's make "Hello world!" render as HTML:

.. literalinclude:: ../../../src/QuickStart/SayHelloController.cs
   :lines: 12-21
   :linenos:

One last time, but this time let's add a title to the page and turn the text
blue. This is the point where I don't care to use raw strings like we just did,
so we're going to switch to using the HtmlTags library. Add this class to your
project and compile:

.. literalinclude:: ../../../src/QuickStart/SayHelloController.cs
   :lines: 23-44
   :linenos:

Compile and open your browser to ~/bluehello:

.. image:: images/getting_started/BlueHello.png

Say my name!
------------

Just to be slightly more challenging, let's create an endpoint that can display
your name with the URL pattern ``~/my/name/is/{Name}.`` Using FubuMVC's
nomenclature for URL patterns, ``{Name}`` would be a ``RouteInput`` that you
have to supply as part of the URL.

To implement this endpoint, enter this code and compile:

.. literalinclude:: ../../../src/QuickStart/SayHelloController.cs
   :lines: 47-66
   :linenos:

Since **my** name is Jeremy, I'm going to enter the URL "~/my/name/is/Jeremy"
into the browser to get to this page:

.. image:: images/getting_started/MyNameIs.png

How did that stuff get there?
-----------------------------

Open your browser to the main diagnostics page at ~/\_fubu (or click the link
"Diagnostics for your application" from the home page for the application):

.. image:: images/getting_started/Diagnostics.png

Get used to the diagnostics pages (~/\_fubu) because you're going to spend a
lot of quality time with them as you're learning FubuMVC. From here, click
onto the link for "Behavior chains" and look carefully at the bottom four rows
in the table and see what we've got.:

.. image:: images/getting_started/DiagnosticsChains1.png

At no point in the "Hello, world" exercise did we explicitly:

    #. Register the \*Controller classes with FubuMVC.

    #. Inherit from any kind of magic base class.

    #. "Tell" FubuMVC to render a string to the output.

    #. "Tell" FubuMVC to render the output of the HelloWorld2Html () method
       with the mimetype "text/html".

    #. Define a route for the get_my_name_is_Name (NameModel input) method
       with the pattern "my/name/is/{Name}" that only responds to the HTTP
       "GET" method.


FubuMVC uses conventions very heavily to figure out what it should do. The
FubuRegistry class dropped in by NuGet registers a few basic conventions to get
you started:

.. CODE[1,27]. src/QuickStart/ConfigureFubuMVC.cs
.. literalinclude:: ../../../src/QuickStart/ConfigureFubuMVC.cs
   :lines: 1-27
   :linenos:

In the code above, there's a convention registered to add all the public
methods in the classes in the QuickStart assembly named [Something]Controller
as actions. There's another set of conventional rules about how to determine a
route based on an action's signature.

Out of the box, FubuMVC has a few simple conventions baked in, and you can see
some of them in play in our "Hello, world!" exercise:

    * If an action returns a string, write that string to the output.

    * If an action method returns a string **and** its name ends in "HTML",
      write out the string returned with the mime type "text/html" and
      remove the "HTML" from the conventionally determined route pattern.

    * If an action method contains underscores, treat the underscores as
      slashes in the generated route pattern. If the action method name
      starts with the name of an HTTP method like "get" or "post," add a
      constraint to the Route.

.. note::

    This guide is concentrating on using conventions to configure the
    application, but it's perfectly possible to use the normal litany of Fluent
    API's and/or .NET Attributes to override the conventions.

What's next?
============

In this guide you've seen how to:

    *  Set up your first FubuMVC application

    *  Add an action, models, and a view

    *  Launch the app, view the diagnostics, and the output of your action

Since this is a getting started guide, there's a lot we did *not* cover.
This getting started app isn't very useful right now.

Next, you'll probably want to:

    *  Use the Spark view engine
    *  Use the WebForms view engine
    *  Control how your Routes are generated and URL's are resolved
    *  Create Ajax endpoints
    *  Wring more value out of FubuMVC's composable model binding
    *  Explore the considerable view helper support
    *  Get you some HTML convention awesomeness
    *  Create custom Wrapper behaviors and apply them
    *  Composing your application with Partials, Content Extensions, and
    *  Bottles
    *  Use the advanced diagnostics
    *  Explore the authorization integration
    *  Learn a little bit about the philosophy and design behind FubuMVC and
       why it's different from other MVC frameworks (and why this helps you)
    *  Configure your own FubuRegistry to start setting up your own
       conventions
    *  Discover what conventions are available to you
    *  Adding functionality in a convention and compositional way
       (harnessing all of FubuMVC's power)
    *  Embrace and extend FubuMVC

(these will be turning into links as we get more guides done)
