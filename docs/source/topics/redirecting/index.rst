======================================================
How do I redirect from a controller action in FubuMVC?
======================================================

.. toctree::
   :maxdepth: 2

By default, FubuMVC provides two ways to redirect to other action calls.  The
redirection feature is provided by ContinuationHandlerConvention, which is
applied by the FubuRegistry.DefaultConventions().

.. note::

  ContinuationHandlerConvention inserts the ContinuationHandler behavior
  directly after the action call if the output is a FubuContinuation or an
  instance of IRedirectable.

Redirecting With FubuContinuation
---------------------------------

For scenarios where you will always be redirecting from the action in some way
or another, you can simply return FubuContinuation from your controller action.

.. note::

  FubuContinuation is similar to the RedirectToRouteResult from ASP.NET MVC.

FubuContinuation has a number of factory methods for your convenience to help
construct them, they are as follows:

FubuContinuation.RedirectTo(object destination)

FubuContinuation.RedirectTo<T>()
  T is the input model of destination action call

FubuContinuation.RedirectTo<T>(Expression<Action<T>> expression)
  T in this case is the type of Controller you want to go to
  In the expression, you can point at the action you want invoked
  Example: FubuContinuation.RedirectTo<Controller>(x => x.Action())

.. warning::

  This expression is a convenience to point at where you want to go, if you
  provide an actual input model instance it will not be respected, would only
  recommend using this when your action takes no input model.

With RedirectTo, a new http request will be made to the destination action call,
which will result in the request data being lost unless persisted trough:

* IFlash (link)
* Session / cookies

FubuContinuation.TransferTo(object destination)
FubuContinuation.TransferTo<T>()
FubuContinuation.TransferTo<T>(Expression<Action<T>> expression)

These overloads are similar to their RedirectTo counterparts
TransferTo stays on the same request.  All request data that was previously
accessible will still be available.

FubuContinuation.EndWithStatusCode<T>()
  Immediately writes the response code to the output stream

FubuContinuation.NextBehavior()
  Does not redirect or transfer, merely passes control on to the next behavior
  in the chain. The equivalent of not having done a redirection in the first
  place.

Example usage:

.. code-block:: c#

  public class Controller
  {
    public FubuContinuation ActionOne()
    {
      return FubuContinuation.RedirectTo<ActionTwoInputModel>();
    }

    public ActionTwoOutputModel ActionTwo(ActionTwoInputModel input)
    {
      return new ActionTwoOutputModel();
    }
  }

  public class ActionTwoInputModel
  {
  }

  public class ActionTwoOutputModel
  {
  }

Redirecting With IRedirectable
------------------------------

For other scenarios where you may only be redirecting under some condition and
in other conditions still be responding with an output then you will want to
make your output type implement the IRedirectable interface.

.. code-block:: c#

  public interface IRedirectable
  {
    FubuContinuation RedirectTo { get; set; }
  }

If the output model sets this property, then it will work exactly as described
above with FubuContinuation.  If it does not, then it will default to
FubuContinuation.NextBehavior() letting you continue to your output node if one
is present in your chain. (output nodes like render view, etc)

Example Usage:

.. code-block:: c#

  public class Controller
  {
    public ActionOneOutputModel ActionOne()
    {
      return new ActionTwoOutputModel
      {
        RedirectTo = FubuContinuation.RedirectTo<Controller>(x => x.ActionTwo());
      };
    }

    public ActionTwoOutputModel ActionTwo()
    {
    }
  }

  public class ActionOneOutputModel : IRedirectable
  {
    public FubuContinuation RedirectTo { get; set; }
  }

  public class ActionTwoOutputModel
  {
  }

.. seealso::

  See :ref:`behavior-chains`

.. seealso::

  See :ref:`one-in-one-out` to understand how the output model of a controller
  action is set into the fubu request for you.
