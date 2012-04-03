.. _configuring-actions-in-fuburegistry:

===================================
Configuring Actions in FubuRegistry
===================================

This guide covers configuring, in your FubuRegistry, which actions FubuMVC will
process. After reading it, you should be familiar with configuring:

    * Which assemblies, types, and methods FubuMVC will consider when looking
      for action methods. 
      
    * How FubuMVC will generate routes for each action
    
    * What the output mechanism will be for each action (i.e. HTML, JSON, 
      XML, etc) 
      
    * Which view, if necessary, a given action will use 
    
.. note::

    All the code used in this guide is available under the "src" folder in the
    `FubuMVC repository on GitHub <http://github.com/DarthFubuMVC/fubumvc>`_

This Guide Assumes...
=====================

    * That you already have a basic FubuMVC starter app up and running (if not,
      check out the :doc:`/intro/gettingstarted` Guide). 
      
    * That you're ready to start making some action methods and types and wire
      them up to routes and views through FubuMVC
    
    * That you're planning on using the ASP.NET WebForms View Engine. This guide
      is not yet adapted for Spark or other view engines, but many of the
      techniques are applicable to all view engines. 
      
Setting up your FubuRegistry
============================

The first thing you need to configure FubuMVC is a class somewhere in your web
project that derives from FubuRegistry. You can call this class anything, but a
common convention is to call it ``ProjectNameFubuRegistry`` where *ProjectName*
is your project's name or code-name.

All the configuration for your ``FubuRegistry``-derived class takes place in the
constructor. Your class will need a default constructor for this purpose.
   
.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_MyProjectFubuRegistry.cs
   :lines: 1-12
   :linenos:
   
Action discovery
================

When configuring your ``FubuRegistry``, the first thing you need to tell it is
where your assemblies are and which types and methods should be considered 
"actions".

An *action* is any method which will be executed when a route is requested by
the client browser. Some people like to have one action per .NET class, some
people like to group several actions into a class and call this class a
*Controller*. FubuMVC only cares about action methods and not much about the
containing type.

Assembly location
-----------------

Action assemblies are configured via the ``Applies`` property on 
``FubuRegistry``. FubuMVC will only scan assemblies you tell it to. If you don't
mention it here to FubuMVC, those types won't be considered for action
discovery!

To tell FubuMVC which assemblies to consider, simply type ``Applies`` then a
period inside your constructor and you should see the applicable methods on
this property. The options available are:

    * ``ToThisAssembly()``: The assembly containing your ``FubuRegistry`` class
      (i.e. your web project) 
      
    * ``ToAssembly(_assembly_)``: A specific assembly by name or reference
    
    * ``ToAssemblyContainingType<T>()``: The assembly containing a specific
      well-known type (i.e. ``MyProject.Actions.FooActions``) 
      
Repeated calls to ``Applies`` are cumulative. You can keep calling and
re-calling these methods to build up a list of assemblies for FubuMVC to scan.

Containing Types and Action Methods
-----------------------------------

Now that we have told FubuMVC how to locate the assemblies we care about, you
need to tell it which types in those assemblies contain action, and which
methods on those types FubuMVC should treat as actions. This is accomplished
via the API exposed through the ``Actions`` property on ``FubuRegistry``.

Like the ``Applies`` API, everything on ``Actions`` is cumulative. There are
several methods that start with the name *Include* and several that start with 
*Exclude*. This is because your *Include* calls may include too much and you may
wish to cherry-pick a few types or methods out of the list of actions.

By default, FubuMVC will not look at any types for actions. If you don't 
*Include* any types, you will have no actions! Also, by default, all public
methods on any types which you have included will be considered actions unless
you specify the methods to include with a call to ``IncludeMethods()``.

Here is a simple example of configuring FubuMVC to consider all types whose name
ends with "Controller" and types that implement your project's ``IAction``
interface as action-containing types:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_RegistryExamples.cs
   :lines: 10-12
   :linenos:

FubuMVC requires action methods to be one-model-in/one-model-out (OMIOMO), 
zero-model-in/one-model-out (ZMIOMO), or one-model-in/zero-model-out (OMIZMO).
All input and output models must be reference types (``class``) not value types
(``struct``). If your method doesn't match this criteria, you will get a fatal
error upon app start (an ASP.NET Yellow Screen of Death).

Keep in mind that if you do not specify the methods to be treated as actions
with a call to ``IncludeMethods()`` or ``ExcludeMethods()``, all public methods
will be exposed as actions. It is good practice to avoid having public methods
on action-containing types (or "controllers" if you prefer) that you don't
intend to be actions, unless you have a narrowly-defined convention for action
discovery.

Configuring Routes
==================

However you configure your routes (default, custom, etc) they must be unique by
route URL stub and HTTP method. If you end up somehow with duplicate routes,
FubuMVC will fail on app start with a fatal error (ASP.NET Yellow Screen of
Death).

Basic Route Configuration
-------------------------

Routes, like most things in FubuMVC, are applied conventionally. This means you
can set them explicitly, or configure FubuMVC with rules by which it should
automatically determine routes for actions.

In FubuMVC, routes are the URL stubs by which a given action can be invoked. For
example */people/charlie* might map to the ``Load()`` method on the 
``PeopleAction`` type (or ``Index`` method on the ``PeopleController`` if you
like to use the term "Controller" for action types).

If you don't specify otherwise, FubuMVC will generate a route like this:

``/your/namespace/here/typename/methodname``

This means that the ``Index`` method of the C# class defined in the file 
*YourProject\Controllers\Home\HomeController.cs* would be mapped from the route:
*/yourproject/controllers/home/home/index* (FubuMVC automatically strips out the
text "Controller" if your action-containing type ends with "Controller"). This
is almost certainly not the route you'd want for this action. More than likely
what you want is just */home*.

To configure this, start with the ``Routes`` API hanging off of 
``FubuRegistry``. Almost all the methods on this API are subtractive. They take
away things from the default route pattern. For example, 
``IgnoreControllerFolderName()`` would result in the route 
*/yourproject/controllers/home*. ``IgnoreControllerNamespaceEntirely()`` would
result in the entire namespace (including the action-containing types's folder
name) being removed from the route, resulting in the much more pleasant 
*/home/index*.

Let's say that your project has a standard that you use the ``Index()`` action
as the default for your routes. Thus the /home/index route should really be 
*/home*. This is where ``IgnoreMethodsNamed()`` comes in handy. If you used
that, you'd end up with the route */home*.

Using our examples above, your ``Routes`` configuration would look something 
like:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_RegistryExamples.cs
   :lines: 14-16
   :linenos:

Custom Route Policies
---------------------

If you really just can't stand how the default route generation works in 
FubuMVC, or you need to do something very specific for your circumstances, you
can always override the whole thing with your own custom ``IUrlPolicy``
implementation.

``IUrlPolicy`` implementations are configured as a fall-through. You can add
many different policies, each targeted towards a particular problem or set of
problems you're trying to overcome. 

URL policies will be queried in the order in which they were registered with the
``Routes`` API (first policy added first, last one added is considered last). If
no policy matches, the default policy will be used as a back-stop. In the end,
every action is guaranteed to get a route.

Register custom ``IUrlPolicy`` implementations using the ``UrlPolicy<T>()``
method on the Routes API.

Constraining by HTTP method
---------------------------

Routes are considered unique according to both their URL stub and their HTTP
method. For example, the route /login for GET requests goes to action
``LoginController.Index()`` but the route */login* for POST requests goes to
``LoginController.Login()``. In FubuMVC, these actions and their routes are
considered distinct, unique routes since they occur on different HTTP methods.

HTTP method constraints can and should be applied conventionally. We recommend
you establish your own convention for which methods are **GET** versus **POST**
methods. For example, methods which are supposed to be POST-only are called
``Command()`` or ``Post()``. To configure this convention, use the
``ConstrainToHttpMethod()`` method on the Routes API. Consider this example:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_RegistryExamples.cs
   :lines: 18-19
   :linenos:

Now all action methods in your application that are named "Post()" will be HTTP
POST-only methods.

Conventions are your friend!
----------------------------

We highly recommend that you encapsulate your application's naming and routing
rules into custom ``IUrlPolicy`` implementations rather than having a lot of
bloat and cruft build up in your ``FubuRegistry`` implementation. We also highly
recommend that you create a conventional pattern for locating, naming, and
constraining your action-containing types and your action methods.

For example, you might decide on a convention with these traits:

    * All actions are contained in types that end with "Action" 
      (i.e. HomeAction) 
    
    * All action-containing types have one or two methods: ``Get`` and ``Post``
      or ``Query`` and ``Command``
      
    * The ``Get/Query`` methods are wired as HTTP GET-only methods and 
      ``Post/Command`` methods are wired up as HTTP POST-only 
      
To implement this convention, you could create an ``IUrlPolicy`` implementation
called ``ActionGetAndPostUrlPolicy`` or ``ActionQueryCommandUrlPolicy``, and
then register it using the ``UrlPolicy<T>`` method on the ``Routes`` API.

Here is a possible implementation of ActionQueryCommandUrlPolicy:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_Custom_IUrlPolicy.cs
   :lines: 8-33
   :linenos:

Attaching views to actions
==========================

Many of your actions will return HTML to the the client browser (those that
don't will be covered in the next section). Views are conventionally attached to
actions via the ``TryToAttach()`` method (and nested closure) on the ``Views`` 
API off of ``FubuRegistry``.

FubuMVC will scan all the assemblies you told it about (from the a few sections
ago via the ``Applies`` API) and look for ASP.NET Page classes that derive from
``FubuPage<TOutputModel>``. This means that FubuMVC does not currently support
code-behind-less views, though we are definitely looking to add them at some
point in the near future (if you really want this, we could sure use the help to
implement it! hint hint).

There are, by default, three different ways FubuMVC will try to attach actions
to views. ASP.NET WebForms views must be strongly-typed and derive from 
``FubuPage<TOutputModel>``, where ``TOutputModel`` is the output model of the
action method to which the view will be attached. You can use all three ways,
just two, or just one if you wish. They are applied cumulatively in the order
that they're specified (i.e. it's a fall-through to find the view). There is no
back-stop for wiring up views to actions. If no view could be found for a given
action, FubuMVC will not raise an error. This is because not all actions result
in view-rendering. Some return JSON or XML or other content. If you run into
difficulties getting view matching to work, consider using FubuMVC Diagonostics
(mentioned in the :doc:getting started guide) which may help determine why 
things are not matching up properly.

There are three methods on the API exposed by the nested closure of the 
``TryToAttach`` method on the ``Views`` API. These represent the three default
strategies of matching views to actions in FubuMVC:

    * ``by_ViewModel``: FubuMVC will only consider the output model type of the
      action and try to match it against the ``TOutputModel`` of the WebForm
      page's ``FubuPage<TOutputModel>`` designation. 
      
    * ``by_ViewModel_and_Namespace``: FubuMVC will consider the output model
      type (the same as above) and also match the namespace of the page to the
      namespace of the action-containing type (i.e. controller class). This
      allows you to have multiple views based off the same viewmodel in
      different namespaces without having conflicts or duplicate matches. 
      
    * ``by_ViewModel_and_Namespace_and_MethodName``: Considers all the above
      criteria, but requires that the view itself be named the same as the
      method. For example if your action method name is "Index()", the view
      must be "Index.aspx". This is the most restrictive of all patterns, but
      allows the greatest flexibility for reuse of viewmodels among many
      different views. 
      
If a single strategy finds multiple views that match an action, it will not
attach any of them. It will instead fall through to the next strategy (or not
attach a view at all). For this reason, it is recommended that you specify the
most restrictive strategies first in your call to ``TryToAttach()``.

Here is an example of the ``TryToAttach()`` method and the nested closure with
all three view location strategies activated:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_RegistryExamples.cs
   :lines: 21-26
   :linenos:

Attaching outputs to actions
============================

For actions that you don't want to return an HTML view from (for example a 
JSON or XML request), use the ``Output`` API on your ``FubuRegistry`` class.

The most common use of this API is for wiring up actions to JSON output (for
invocation by your favorite client-side JavaScript framework, such as jQuery).
You can use the ``ToJson`` property on the ``Output`` API for this purpose. 
There are two options: 

    * Attach to an action by an arbitrary criteria or, 
    
    * more commonly, by the action's output model.

From our experience, it's generally better to use the same output type for
marshalling all types of JSON messages back to the client so they can be
processed consistently on the client (for error handling, success handling,
timeout handling, etc).

If your JSON output type is, for example JsonResponse, you could use the 
``WhenTheOutputModelIs<JsonResponse>()`` method. This will instruct FubuMVC to
wire up a JSON output handler for all action methods that return a model object
that can be cast as ``JsonResponse``.

You can also define your own custom output modes using the ``To`` method.
Explaining the use of this method is beyond the scope of this already too-long
guide. You could always use the `mailing list 
<http://groups.google.com/group/fubumvc-devel>`_ if you think this is something
you need, and want some help.

Here is an example of wiring up all actions whose output model is 
``JsonResponse`` as JSON-output actions:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_RegistryExamples.cs
   :lines: 28-29
   :linenos:

Summary
=======

Congratulations! You've made it through this guide.

In this guide you've seen how to :

    * Discover your action assemblies, types, and methods 
    
    * Determine how your actions will be routed via URL and HTTP method 
    
    * How to use ``IUrlPolicy`` to define and encapsulate your project's route
      conventions 
      
    * Match actions to their corresponding views 
    
    * Determine the output type for actions which don't render views 