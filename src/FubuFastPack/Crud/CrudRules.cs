using System;
using FubuMVC.Core.Security;

namespace FubuFastPack.Crud
{
    public class CrudRules
    {
        public static string SecurableNameForCreation(Type type)
        {
            return "Create:" + type.Name;
        }

        public static string SecurableNameForViewing(Type type)
        {
            return "View:" + type.Name;
        }

        public static bool CanCreate(Type type)
        {
            return PrincipalRoles.IsInRole(SecurableNameForCreation(type));
        }

        public static bool CanView(Type type)
        {
            return PrincipalRoles.IsInRole(SecurableNameForViewing(type));
        }

    }
}