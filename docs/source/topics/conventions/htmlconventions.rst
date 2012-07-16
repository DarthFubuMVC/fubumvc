.. _html-conventions:

================
Html Conventions
================

FubuMVC allows one to modify the output of HTML code based on conventions. 

.. _html-convention-registry:

Html Convention Registry
------------------------

In the FubuRegistry there is a HtmlConvention method to include any Html
Convention Registries. 

.. code-block:: c#

  public class TestFubuRegistry : FubuRegistry
  {
    TestFubuRegistry()
    {
      HtmlConvention<TestHtmlConventionRegistry>();
    }
  }

Example HtmlConventionRegistry. This includes:
  * :ref:`modify-for-attribute` 
  * :ref:`optional-element-modifier`
  * :ref:`conditional-building`

.. code-block:: c#

   public class TestHtmlConventionRegistry : HtmlConventionRegistry
   {
     public TestHtmlConventionRegistry()
     {
       Editors.ModifyForAttribute<RequiredAttribute>(x =>
       {
         //Code here
       });
       Editors.Modifier<PasswordHtmlModifier>();
       Editors.If(x => x.Accessor.PropertyType.IsEnum)
        .BuildBy(DropDownBuilder);
     }
   }

.. _modify-for-attribute:

Modify For Attribute
--------------------

The ModifyForAttribute method allows you to create an attribute that you can
place on properties which will modify how the HTML is generated.

Example use of a Required attribute

.. code-block:: c#

  public class TestViewModel
  {
    [Required]
    public string TestValue { get; set; }
  }

During registration you can specify what you want to happen when an input is
created using that Property. The example adds a * after the input.

.. code-block:: c#

   Editors.ModifyForAttribute<RequiredAttribute>(x =>
   {
    x.Append(new HtmlTag("span").Text("*");
   });
  

.. _optional-element-modifier:

Optional Element Modifier
-------------------------

An Optional Element Modifier is a way to conventionally modify your HtmlTags.
For example any Property that contains password the input will automatically
include the type="password" attribute. See an example in :ref:`forms`

.. code-block:: c#

  public class PasswordHtmlModifier : OptionalElementModifier
  {
    public override bool Matches(AccessorDef def)
    {
      return def.Accessor.PropertyType == typeof(string) &&
          def.Accessor.FieldName.EndsWith("password", 
          StringComparison.OrdinalIgnoreCase);
    }

    public override void Build(ElementRequest request, HtmlTag tag)
    {
      var inputTag = tag.Children
      .FirstOrDefault(x => x.TagName().ToLower() == "input");
      if (inputTag != null)
      {
        inputTag.Attr("type", "password");
      }
    }
  }

.. _conditional-building:

Conditional Building
--------------------

The example registration is the same as a :ref:`optional-element-modifier` but
is done in-line rather then its being in its own class.

.. code-block:: c#

   Editors.If(x => x.Accessor.PropertyType.IsEnum)
    .BuildBy(DropDownBuilder);

   //Another example
   //Editors.IfPropertyIs<bool>().BuildBy(CheckBoxBuilder);

The DropDownBuilder is just an example method that builds a select drop-down
input based on the Enum and selects the current value.

.. code-block:: c#

   public static HtmlTag DropDownBuilder(ElementRequest request)
   {
     Array values = Enum.GetValues(request.Accessor.PropertyType);
     return new SelectTag(tag =>
     {
       foreach (var option in values)
       {
         tag.Option(option.ToString(), (int) option);
       }
       tag.SelectByValue(((int) request.RawValue).ToString());
     });
   }
