==============
Know your role
==============

One way to control access rights is ensure the authenticated user is in the
correct role.

Meet the AllowRoleAttribute
---------------------------

The AllowRoleAttribute will restrict all access to a list of roles.  If the user does
not happen to be in any of the roles listed, then a response with a 403 status
code will be returned.  Otherwise, the user will be allowed to proceed as
normal.

Example usage:

.. code-block:: c#

  [AllowRole("CanI")]
  public class Controller
  {
      [AllowRole("MayI", "AllowMe")]
      public void RestrictedAction(InputModel input)
      {
          //logic
      }
  }

Also note that the attribute can be applied on the controller level as well as
the action level.

Using attributes may be fine for those one-off cases, but imagine working on a
web application designed with role-based access control (RBAC) in mind.  With
this type of control, every action can potentially be restricted to one or many
roles.  This can quickly become a maintenance nightmare when trying to restrict
all of those actions.  It sure would be nice to have some sort of convention set
up for this...

Using a custom authorization policy
-----------------------------------

Let's define a convention that every action taking an input type with its name
ending in "RoleModel" will be restricted to a particular role.  The role that
the action is allowed for will be the name of the input type without the
"RoleModel" suffix.  For example, the UpdateDescriptionRoleModel will restrict
a user who is in the "UpdateDescription" role.

.. code-block:: c#

  public class RoleModelAuthorizationPolicy : IConfigurationAction
  {
      public void Configure(BehaviorGraph graph)
      {
          graph.Actions()
              .Where(x => x.HasInput && x.InputType().Name.EndsWith("RoleModel"))
              .Each(x =>
              {
                  var roleName = x.InputType()Name.Replace("RoleModel", string.Empty);
                  x.ParentChain().Authorization.AddRole(roleName);
              });
      }
  }

The controller action would look something like this.

.. code-block:: c#

  public class InventoryItemController
  {
      public void ChangeDescription(UpdateDescriptionRoleModel input)
      {
          //logic
      }
  }
