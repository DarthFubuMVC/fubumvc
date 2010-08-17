using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Registration.ObjectGraph
{
    public class ListDependency : IDependency
    {
        public ListDependency(Type dependencyType)
        {
            Items = new List<ObjectDef>();
            DependencyType = dependencyType;
        }

        public Type DependencyType { get; private set; }

        public IList<ObjectDef> Items { get; private set; }

        public Type ElementType
        {
            get
            {
                if (DependencyType.IsArray) return DependencyType.GetElementType();


                return DependencyType.GetGenericArguments()[0];
            }
        }

        public void AddValue(object value)
        {
            Items.Add(new ObjectDef(){
                Value = value
            });
        }

        public ObjectDef AddType(Type type)
        {
            var objectDef = new ObjectDef(type);
            Items.Add(objectDef);

            return objectDef;
        }


        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.List(this);
        }


    }
}