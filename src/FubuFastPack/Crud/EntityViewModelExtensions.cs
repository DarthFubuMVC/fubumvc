using System;
using FubuCore;

namespace FubuFastPack.Crud
{
    public static class EntityViewModelExtensions
    {
        public static Type GetEntityModelType(this Type modelType)
        {
            var entityModelType = modelType.FindInterfaceThatCloses(typeof(IEntityViewModel<>));

            if (entityModelType != null)
            {
                modelType = entityModelType.GetGenericArguments()[0];
            }

            return modelType;
        }
    }
}