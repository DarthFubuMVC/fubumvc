using System;
using System.Collections.Generic;
using FubuCore;
using FubuFastPack.Crud.Properties;
using FubuFastPack.Security;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.Querying;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Core.Security;
using FubuMVC.Core.Urls;

namespace FubuFastPack.Crud
{
    public class CrudActionModifier
    {
        private readonly Type _entityType;
        private readonly Type _editModelType;
        private readonly string _routeName;

        public CrudActionModifier(Type handlerType)
        {
            _entityType = handlerType.GetEntityType();
            _editModelType = handlerType.GetEditEntityModelType();

            _routeName = _entityType.Name.ToLower();
        }

        public void ModifyChains(HandlerActionsSet actions, BehaviorGraph graph)
        {
            // Attach the creation handlers
            actions.ForOutput(x => x.Closes(typeof(CreationRequest<>))).Each(addCreatorCall);

            // Attach the creation permissions
            actions.StartingWith("Create").Each(addCreationPermission);
            actions.StartingWith("New").Each(addCreationPermission);

            // Mark the New method as the 'new' url for that entity type
            modifyNewAction(actions);

            // Flesh out the behavior chains for "New" chains
            actions.StartingWith("New").Each(addNewEntityPipeline);

            // add the Find and EditProperty endpoints
            addEndpointsFor(_entityType, graph);

            // Attach more behaviors and security for the Edit endpoints
            modifyEditAction(actions.ByName("Edit"));
        }

        private void modifyEditAction(ActionCall action)
        {
            // At least one Crud controller 'ignores' its Edit method
            if (action == null) return;

            var chain = action.ParentChain();
            chain.Route = action.BuildRouteForPattern("{0}/{{Id}}".ToFormat(_routeName));

            // If there are no 
            if (!action.HasAttribute<AuthorizationAttribute>())
            {
                var permissionName = CrudRules.SecurableNameForViewing(_entityType);
                chain.Authorization.AddRole(permissionName);
            }

            // apply data restrictions
            var policyType = typeof(RestrictedDataAuthorizationPolicy<>).MakeGenericType(_entityType);
            chain.Authorization.AddPolicy(policyType);

            action.AddAfter(Wrapper.For<CrudUrlBehavior>());
        }

        private void addNewEntityPipeline(ActionCall action)
        {
            var lastAction = action;

            if (action.OutputType() == _entityType)
            {
                var handlerType = action.HandlerType;
                var editMethod = handlerType.GetMethod("Edit");
                var editPass = new ActionCall(handlerType, editMethod);
                action.AddAfter(editPass);

                lastAction = editPass;
            }

            lastAction.AddAfter(Wrapper.For<CrudUrlBehavior>());
        }

        private void modifyNewAction(HandlerActionsSet actions)
        {
            var newAction = actions.ByName("New");
            if (newAction == null) return;
            // Set the url categorization and the route
            var chain = newAction.ParentChain();
            chain.UrlCategory.Creates.Add(_entityType);
            chain.Route = new RouteDefinition(_routeName + "/new");
        }

        private void addCreationPermission(ActionCall action)
        {
            // If there are no other permissioning, add one
            if (!action.HasAttribute<AuthorizationAttribute>())
            {
                var permissionName = CrudRules.SecurableNameForCreation(_entityType);
                action.ParentChain().Authorization.AddRole(permissionName);
            }
        }

        private static void addEndpointsFor(Type entityType, BehaviorGraph graph)
        {
            var findUrlPattern = "{0}/find/{{Id}}".ToFormat(entityType.Name.ToLower());
            graph.AddActionFor(findUrlPattern, typeof(DomainEntityFinder<>), entityType)
                .UrlCategory.Category = Categories.FIND;

            var finderForwarder = typeof(EntityFinderForwarder<>).CloseAndBuildAs<IChainForwarder>(entityType);
            graph.AddForwarder(finderForwarder);


            var editPropertyUrlPattern = "{0}/editproperty".ToFormat(entityType.Name).ToLower();
            graph.AddActionFor(editPropertyUrlPattern, typeof(IPropertyUpdater<>), entityType)
                .UrlCategory.Category = Categories.PROPERTY_EDIT;

            var propertyForwarder = typeof(PropertyUpdaterForwarder<>).CloseAndBuildAs<IChainForwarder>(entityType);
            graph.AddForwarder(propertyForwarder);
        }



        private void addCreatorCall(ActionCall call)
        {
            var actionType = typeof(EntityCreator<,>).MakeGenericType(_editModelType, _entityType);
            var method = actionType.GetMethod("Create");

            var createAction = new ActionCall(actionType, method);
            call.AddAfter(createAction);

        }


    }

}