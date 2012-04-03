.. _advanced-behaviors:

==================
Advanced Behaviors
==================

This guide covers more advanced scenarios for configuring and applying behaviors
you've already written to an existing application. After reading it, you should
be familiar with:

    * How to apply a behavior to all actions
    
    * How to apply a behavior to selective actions
    
    * How to create your own conventions for applying behaviors
    
.. note::

    All the code used in this guide is available under the "src" folder in the
    `FubuMVC repository on GitHub <http://github.com/DarthFubuMVC/fubumvc>`_

This Guide Assumes...
=====================

    * You've at least read through the 
      :doc:`/intro/gettingstarted` guide. 

    * You have a FubuMVC project already up and running and serving content
    
    * You're familiar with :doc:`behaviors` in general and already have one or
      more written. 
      
    * You already have a FubuRegistry class configured and ready to go 
    
    * You know that a *Behavior* is a type that implements +IActionBehavior or
      inherits from the convenience base class BasicBehavior. 
      
Behavior Responsibilities
=========================

Before we begin this guide, it's important to understand some basic terminology.
Behaviors are "chained" together at config/bootstrap-time. Behaviors are chained
together by injecting the "inside" (or "next") behavior into its parent or 
previous behavior in the chain.

Behaviors are responsibile for calling their inner behavior (the next link in 
the chain) unless the purpose of the behavior is to ursurp the processing of the
chain and handle the request entirely.

A behavior has full power over the call to its inner link. This means the 
behavior can:

    * Execute code before calling the inner behavior 
    
    * Execute code after calling the inner behavior 
    
    * Execute code around calling the inner behavior (i.e. a using() block or a
      try/finally block) 
      
    * Execute the inner behavior and then decide to do something different. For
      example, if the inner behavior throws an error or does something that 
      triggers the outer behavior to respond differently (such as redirecting to
      another action in the case of a successful login). 

    * Not call the inner behavior at all and do something completely different 
      (return a cached result to the browser, redirect to the login page, 
      display a "Site down" message, etc) 
      
As you can see, this complicates matters when we talk about behaviors executing 
"before" or "after" other behaviors because it's not quite that simple.

Generally speaking, most behaviors will either wrap the inner behavior or
execute code before or after. For the sake of this guide, when the terms "wrap",
"before", "after", and other forms of describing behaviors in a linear chain, it
is these types of general behaviors that are being described.

Policies
========

In your FubuRegistry class, there is a "Policies" property with a fluent
interface exposed from it that you can use to control how behaviors are 
configured and applied and in which order.

Generally speaking, the order of how behaviors are specified in Policies is the
order which they will be wrapped. That is, the first behavior specified will be
the inner-most behavior.

Each successive call to ``WrapBehaviorChainsWith<T>``, for example, will wrap
the previous behavior.

For example, this configuration:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_BehaviorChainExamples.cs
   :lines: 10-12
   :linenos:
            
will result in ``OuterBehavior`` being called, then ``MiddleBehavior``, then 
``InnerMostBehavior``, and finally the action.

Applying a behavior to all actions
==================================

To apply a behavior to all actions, simply call the 
``WrapBehaviorChainsWith<T>()`` method as demonstrated in the previous chapter
above.

As mentioned, the order you call ``WrapBehaviorChainsWith<T>()`` will determine
the ordering of behaviors. Remember: the calls at the top are the **innermost**
and the calls at the bottom will be the **outermost**.

Applying a behavior to selected actions
=======================================

There are two ways of doing applying behaviors to selected actions:
 
    1) calling EnrichCallsWith<T>(...) and 
    2) supplying your own convention. This chapter will cover the first case 
       (EnrichCallsWith).

``EnrichCallsWith`` takes in a generic parameter T that is the behavior and an 
``Func<ActionCall, bool>`` delegate which serves as a filter for the actions.
This filter determines to which actions this behavior will be applied. 
``ActionCall`` is part of FubuMVC's configuration model that represents an 
action method (and related information) that was discovered during the "Action"
phase of FubuMVC's  configuration and discovery. 
With the ``ActionCall``, you can access the ``MethodInfo`` of the action method
itself, its declaring type, the input model type (if present), the output model
type, and a few other interesting bits of information about this particular
action.

For example, let's say you wanted to apply this behavior to actions whose method
name is "Home". You would specify it like this:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_BehaviorChainExamples.cs
   :lines: 14-15
   :linenos:
    
And if you wanted to restrict the application of this behavior only to actions
that returned HomeViewModel, you could specify it like this:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_BehaviorChainExamples.cs
   :lines: 17-18
   :linenos:

Creating Your Conventions for Applying Behaviors
================================================

There are some good reasons why you might not want to always use 
``EnrichCallsWith`` such as: 

    * Your condition logic is a little too complicated to have in your 
      FubuRegistry class and won't be easy to test 
      
    * or maybe you need lower-level access to the behavior chain to manipulate
      the ordering of behaviors to insert, remove, or re-arrange behaviors.

For these "heavier lifting"" scenarios, consider adding a behavior convention
class. To do this, first create a new class that implements the 
``IConfigurationAction`` interface (from the ``FubuMVC.Core.Registration`` 
namespace) and add the ``Configure(BehaviorGraph)`` method.

``BehaviorGraph`` is the full FubuMVC configuration model. From this class you
can access everything FubuMVC currently knows about and has configured. 
(ie. All BehaviorChains, services, and various other bits.)

For the sake of this chapter, imagine you wish to wrap all action whose method
name is "Home". This is exactly the same functionality that 
``WrapBehaviorChainsWith<T>`` exposes, by the way. But to illustrate how the 
``BehaviorGraph`` and chains work, this example is simple and sufficient.

To carry out this example, you will need to:

    * Scan the behavior graph for chains that contain an ``ActionCall`` whose
      method name is "Home"
      
    * Loop over chain and "prepend" a Wrapper ``BehaviorNode`` to this chain and 
      specify that the wrapper should wrap the inner behaviors with the desired
      behavior (in this example, ``DemoBehaviorForSelectActions``) 
      
Here is what your Configure method should look like:

In your FubuRegistry, in the Policies section:

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Examples/configuring_actions_BehaviorChainExamples.cs
   :lines: 20
   :linenos:

And then, in your policy class (DemoBehaviorPolicy in this case):

.. literalinclude:: ../../../../src/FubuMVC.GuideApp/Behaviors/DemoBehaviorPolicy.cs
   :lines: 6-16
   :linenos:
   
``BehaviorGraph`` is the whole FubuMVC configuration model. It contains a 
collection of individual ``BehaviorChains`` which represent what will be
eventually configured in the IoC container as the chain of behaviors associated
with a particular route. Behavior chains contain ``BehaviorNodes`` which are
config-time model items that represent which behaviors and actions will actually
be wired up when the FubuMVC configuration is sealed into the IoC container.
Most behavior chains will eventually result in an action method getting 
executed, though they can be configured with a list of actions to call (i.e. a 
full action, or a JSON-only action, or a mobile-browser action).

The various "Node" classes (i.e. ``BehaviorNode``, ``OutputNode``, ``Wrapper``)
are part of FubuMVC's config-time model. The FubuRegistry builds up the 
``BehaviorGraph`` which has ``BehaviorChains`` and services. ``BehaviorChains``
have a collection of ``BehaviorNodes`` and ``ActionCalls``. When config-time is
done and FubuMVC's configuration is being sealed into the IoC container, the
configuration model is then translated into the appropriate run-time
representation of the necessary object dependency graph (the practical result of
this varies from IoC container to IoC container).

When you have access to the ``BehaviorGraph`` at config time, you can do almost
anything with FubuMVC's configuration:

    * Add whole routes/chains/actions,
    
    * re-configure chains, 
    
    * re-arrange/re-order behaviors in a chain, 
    
    * change the route of a chain, 
    
    * change the action that gets called for a particular chain, etc.
    
In fact, the way the FubuDiagnostics work is to insert a special 
*diagnostics/recording* behavior around every behavior in every chain currently
configured. This can be seen in the FubuMVC diagnostics screen.

Summary
=======

In this guide we showed you how to apply behaviors to every action, some 
actions, and some actions using complicated logic. We also talked a little about
how FubuMVC's config-time model works and the power exposed with behavior
policies using ``IConfigurationAction``.
