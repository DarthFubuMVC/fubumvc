using System;
using FubuMVC.Core;

namespace FubuMVC.GuideApp.Examples
{
    public class configuring_actions_RegistryExamples : FubuRegistry
    {
        public configuring_actions_RegistryExamples()
        {
            Actions
                .IncludeTypesNamed(x => x.EndsWith("Controller"))
                .IncludeTypesImplementing<IAction>();

            Routes
                .IgnoreControllerNamespaceEntirely()
                .IgnoreMethodsNamed("Index");

            Routes.ConstrainToHttpMethod(
                x => x.Method.Name == "Post", "POST");

            Views.TryToAttach(x =>
                {
                    x.by_ViewModel_and_Namespace_and_MethodName();
                    x.by_ViewModel_and_Namespace();
                    x.by_ViewModel();
                });

            Output
                .ToJson.WhenTheOutputModelIs<JsonResponse>();

        }
    }


    public class JsonResponse{}

    public interface IAction{}
}