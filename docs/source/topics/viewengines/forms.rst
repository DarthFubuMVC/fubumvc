.. _forms:

=====
Forms
=====

FubuMVC makes it easy to apply ModelBinding to your HTML Forms. The only
requirement on your forms is the names on the inputs must match the name of your
controllers Input Model.

Example Input Model.

.. code-block:: c#

  public class TestInputModel
  {
    public string TestValue { get; set; }
    public string Password { get; set; }
  }

.. note::

    FubuMVC includes many useful extension methods to help make your
    ModelBinding easier.

This Spark code

.. code-block:: html

  ${ this.FormFor<TestInputModel>() }
    ${ this.InputFor<TestInputModel>(m => m.TestValue) }
    ${ this.InputFor<TestInputModel>(m => m.Password) }
    <input type="submit" value="Submit" /> 
  ${ this.EndForm() }
  
will output this HTML. The password added the type="password" because of an
:ref:`optional-element-modifier`.

.. code-block:: html

  <form method="post" action="/test">
    <input name="TestValue" />
    <input name="Password" type="password" />
    <input type="submit" value="Submit" /> 
  </form>

Example Controller that will return the TestValue that was entered into the text
box.

.. code-block:: c#

  public class TestController
  {
    public string Test(LoginInputModel input)
    {
      return input.TestValue;
    }
  }
