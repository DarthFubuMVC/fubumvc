=======================
Coming from ASP.NET MVC
=======================

Controllers
===========

There is no requirement for base class. FubuMVC allows you to define your own
conventions. Such as classes that end with the name Controller. Other
Conventions to take a look at are Handler and EndPoint conventions.

Action Results
==============

FubuMVC doesn't require the Controller to tell us what the output should be.  We
return a single ViewModel that can be represented in multiple output formats
like: HTML, JSON, XML, CSV, etc.

   * System.Web.Mvc.ContentResult
      return a string from your controller action
   * System.Web.Mvc.EmptyResult
      make the return type of the method void
   * System.Web.Mvc.FileResult
      return a DownloadFileModel (be sure to register the DownloadFileConvention in your FubuRegistry)
   * System.Web.Mvc.HttpUnauthorizedResult
      return from your controller action an HttpStatusCode.Unauthorized
   * System.Web.Mvc.JavaScriptResult
      you don't need them; see :ref:`asset-pipeline`
   * System.Web.Mvc.JsonResult
      define a convention to output your ViewModel as JSON. Alternatively see :ref:`conneg`
   * System.Web.Mvc.RedirectResult
      return a FubuContinuation from your controller action
   * System.Web.Mvc.RedirectToRouteResult
      return a FubuContinuation from your controller action
   * System.Web.Mvc.ViewResultBase
      see the section on :ref:`view-engines`

View File Organization
======================

FubuMVC's shared directory for layouts and partials is at the root of the host.
Aside from that, you can locate your views the same way as ASPMVC however we
encourage you to try out other means of organizations. See :ref:`view-engines`

Action Filter
=============

A common requirement to execute code before and after a Controller action
executes. In FubuMVC this is accomplished with Behaviors :ref:`behaviors`

Render Action
=============

A way to invoke an additional controller action from a View template. Partial
requests :ref:`ui-helpers`

Action Link
===========

A way to generate an approptiate hyperlink for a Controller Action.
:ref:`ui-helpers`

Defining Routes
===============

No need to explicitly define route patterns in FubuMVC. However, to customize
how routes are generated see :ref:`customizing-routes`

Route Contraints
================

Although one can define conventions in FubuMVC by using attributes similar to
ASP MVC we encourage one to define less intrusive conventions which, out of the
box, can be as simple as prefixing one's Controller Method Name with the verb.
For more information see :ref:`customizing-routes`.

Using Containers
================

FubuMVC from the ground up is built with IOC. Currently the only container
support is StructureMap. However, it is built agnostic and could be adapted to
any container of choice (with a little work). There's no need to provide
controller factories, etc.

Session State
=============

In your controller's constructor bring in ISessionState.

TempData
========

In your controller's constructor bring in IFlash. In FubuMVC's implementation
the value is serialized as JSON. This decouples the Writer from the Reader if
necessary.
