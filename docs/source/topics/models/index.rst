.. _model-binding:

===================================================
Reading Forms, QueryString, Cookies (Model-Binding)
===================================================

.. toctree::
   :maxdepth: 2

ValueConverter
--------------

A ValueConverter is the lowest level of binding. It is how you can convert raw
data into a different resulting type. The best example is taking a string, and
converting it directly into a more usable type such as a DateTime, TimeZoneInfo,
domain entity, etc.

Value converters are generally stateless. They are able to take the raw value,
and a reference to the PropertyInfo that will be set and perform the conversion
and return the converted value. For these types of conversions, it is
recommended that you inherit from StatelessConverter.

An example of a StatelessConverter can be found in the `FubuCore
<https://github.com/DarthFubuMVC/fubucore>`_ project

.. code-block:: csharp

   public class BooleanFamily : StatelessConverter
   {
       private static TypeConverter _converter = TypeDescriptor.GetConverter(typeof(bool));
       public const string CheckboxOn = "on";

       public override bool Matches(PropertyInfo property)
       {
           return property.PropertyType.IsTypeOrNullableOf<bool>();
       }

       public override object Convert(IPropertyContext context)
       {
           var rawValue = context.RawValueFromRequest.RawValue;

           if (rawValue is bool) return rawValue;

           var valueString = rawValue.ToString();

           return valueString.Contains(context.Property.Name)
               || valueString.EqualsIgnoreCase(CheckboxOn)
               || (bool)_converter.ConvertFrom(rawValue);
       }
   }

You may also have a stateful converter. These converters are rare, but if you
find a case where it is necessary, you will need to create a corresponding
IConverterFamily implementation. These types will have to be registered in
StructureMap and the scanning will pick them up automatically. One caveat with
this is that order of registration matters. The first one in in wins, so ensure
you register your converters in order.

IPropertyBinder
---------------

These are meant for binding properties on your models to something not in the 
IRequestData bag. That is, something that wasn't posted to the server or found 
in your ASP.NET Request or the AppSettings, etc. Property binders are typically
based more on conventions than data type. For example, if you wanted to bind all
properties whose name ends in a certain word, or are decorated with a certain
attribute, you would use an IPropertyBinder.

* CurrentUserBinder -- If a property is of type User (our domain entity
  representing a user of the system) and has the name "CurrentUser", then
  automatically set it to the currently logged-on user from the current
  Principal.

* CurrentTimeZoneBinder -- If a property is named "CurrentTimeZone" and is of
  type TimeZoneInfo, then bind it to the current logged-on user's selected
  time zone.

* CurrentEmailAddressBinder -- You probably get the gist now. If it's
  "CurrentEmailAddress" and of type string, set it to the current logged-on
  user's email address.

You can create your own property binders by implementing the IPropertyBinder
interface:

.. code-block:: csharp

   public class CurrentDatePropertyBinder : IPropertyBinder
   {
       public bool Matches(PropertyInfo property)
       {
           return property.PropertyType == typeof(DateTime) 
               && property.Name == "CurrentDate";
       }

       public void Bind(PropertyInfo property, IBindingContext context)
       {
           property.SetValue(context.Object, DateTime.Now, null);
       }
   }

In this example, the property binder is checking if any property on a model is a
DateTime and has the name CurrentDate. If the property matches, the Bind method
will set that property value to DateTime.Now. 

IModelBinder
------------

This is used if you need something beyond binding model objects and properties to 
name/value pair data.  You can apply IModelBinders to specific situations 
or complete replacement of Fubu's default, built-in StandardModelBinder. It does
NOT require a full replacement/override of huge swaths of the model binding
framework to handle a one-off situation.  You can do it piece by piece. However,
it does have the flexibility to be completely replaced/swapped-out if you need
to do something completely different than name/value pair binding.

Imagine an IModelBinder implementation called "EntityModelBinder". This will
turn the raw value of a Guid in string format (i.e.
``8205556c-2949-4a99-8373-82114004342c``) into a Domain Entity. Since we're in a
web context, imagine a form post variable named "RelatedCase" and the value for
it was ``8205556c-2949-4a99-8373-82114004342c``. Imagine our input model has a
property of type Case (a domain entity) named RelatedCase. The EntityModelBinder
will take that raw Guid string and look at the property type (Case) and attempt
to load a Case entity from the database using that Guid as the primary key.

