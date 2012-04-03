.. _behavior:

=========
Behaviors
=========

This guide covers how to use behaviors to layer or compose functionality in your
application without having to add a lot of extra unrelated code to your actions
or decorate them with attributes. After reading it, you should be famiilar with:

    * What a behavior is
    
    * How behaviors are used in FubuMVC
    
    * (TODO: This guide needs to be fleshed out.)

.. note::

    All the code used in this guide is available under the "src" folder in the
    `FubuMVC repository on GitHub <http://github.com/DarthFubuMVC/fubumvc>`_

This Guide Assumes...
=====================

    * You've at least read through the 
      :doc:`/intro/gettingstarted` guide. 

    * You have a FubuMVC project already up and running and serving content
    
    * You already have a FubuRegistry class configured and ready to go 
    
What is a behavior
==================

A "Behavior" is a type that implements ``IActionBehavior`` or inherits from the
convenience base class ``BasicBehavior``. 

A Behavior is part of an "Action" pipeline of small, compose-able units of
functionality. Each should have its own small responsibility. Each should have
the ability to wrap the entire pipeline and therefore be able to execute before,
after, or around the "Action".

The ``IActionBehavior`` simply defines two Actions:

.. literalinclude:: ../../../../src/FubuMVC.Core/Behaviors/IActionBehavior.cs
   :lines: 8-12
   :linenos:

The interface's second method, ``InvokePartial``, allows you the option of
altering the behavior when it is invoked in a *partial*.

(TODO: provide a link to a document describing partials) 

Since Behaviors are more than just ActionFilters and ActionResults, they can
hook into and override any part of the pipeline.  This allows us to implement
many interesting scenarios and provides maximum flexibility when building a
complex, compositional application.

Since everything in FubuMVC runs through IoC (TODO: Link to InversionOfControl)
a behavior takes in the next behavior in the chain, and has the option of
executing it or not.  If the behavior chooses not to execute the next Behavior
in the chain, it takes responsibility for completing the entire request.

For example:

    * An authorization Behavior would need to usurp the pipeline when the user
      doesn't have the correct permissions to view the page.  This behavior will
      send the request down another pipeline that results in an "HTTP 403 Access
      Denied" view being rendered.
      
BasicBehavior Convienience class
--------------------------------

There also exists an abstract base class, ``BasicBehavior`` that you have the
option of deriving from.  This class takes care of some of the boring basic
behavior stuff.

(TODO Questions that need answering)

    * What are some things I can do with a behavior?
    
    * Why would I want to inherit from ``BasicBehavior`` instead of implementing
      ``IActionBehavior``

    * (TODO: Waiting on content...)
    
Summary
=======

In this guide we explained what a behavior is. 
(TODO: There is more to come.)
